using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Wang_Gloria_HW5.DAL;
using Wang_Gloria_HW5.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace Wang_Gloria_HW5.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.Include(s=> s.Suppliers).Include(d => d.OrderDetails).ThenInclude(o => o.Order).ToListAsync());
        }

        // GET: Products/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            //id was not specified - show the user an error
            if (id == null)
            {
                return View("Error", new String[] { "Please specify a product to view!" });
            }

            //find the course in the database
            //be sure to include the relevant navigational data
            Product product = await _context.Products.Include(s => s.Suppliers).Include(d => d.OrderDetails).ThenInclude(o => o.Order)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            // product was not found in the database
            if (product == null)
            {
                return View("Error", new String[] { "That product was not found in the database." });
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.AllSuppliers = GetAllSuppliers();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,Name,Description,Price,Producttype")] Product product, int[] SelectedSuppliers)
        {

            //This code has been modified so that if the model state is not valid
            //we immediately go to the "sad path" and give the user a chance to try again
            if (ModelState.IsValid == false)
            {
                //re-populate the view bag with the suppliers
                ViewBag.AllSuppliers = GetAllSuppliers();
                //go back to the Create view to try again
                return View(product);
            }

            //if code gets to this point, we know the model is valid and
            //we can add the product to the database

            //add the product to the database and save changes
            _context.Add(product);
            await _context.SaveChangesAsync();

            //add the associated suppliers to the product
            //loop through the list of supplier ids selected by the user
            foreach (int supplierID in SelectedSuppliers)
            {
                //find the supplier associated with that id
                Supplier dbSupplier = _context.Suppliers.Find(supplierID);

                //add the supplier to the product's list of supplier and save changes
                product.Suppliers.Add(dbSupplier);
                _context.SaveChanges();
            }
            

            //Send the user to the page with all the suppliers
            return RedirectToAction(nameof(Index));

        }

        // GET: Products1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //if the user didn't specify a product id, we can't show them 
            //the data, so show an error instead
            if (id == null)
            {
                return View("Error", new string[] { "Please specify a product to edit!" });
            }

            //find the product in the database
            //be sure to change the data type to product instead of 'var'
            Product product = await _context.Products.Include(s => s.Suppliers).FirstOrDefaultAsync(s => s.ProductID == id);

            //if the product does not exist in the database, then show the user
            //an error message

            if (product == null)
            {
                return View("Error", new string[] { "This product was not found!" });
            }

            //populate the viewbag with existing Suppliers
            ViewBag.AllSuppliers = GetAllSuppliers(product);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("ProductID,Name,Description,Price,Producttype")] Product product, int[] SelectedSuppliers)
        {
            //this is a security check to see if the user is trying to modify
            //a different record.  Show an error message
            if (id != product.ProductID)
            {
                return View("Error", new string[] { "Please try again!" });
            }

            if (ModelState.IsValid == false) //there is something wrong
            {
                ViewBag.AllSuppliers = GetAllSuppliers(product);
                return View(product);
            }

            //if code gets this far, attempt to edit the product
            try
            {
                //Find the product to edit in the database and include relevant 
                //navigational properties
                Product dbProduct = _context.Products
                    .Include(c => c.Suppliers)
                    .FirstOrDefault(c => c.ProductID == product.ProductID);

                //create a list of suppliers that need to be removed
                List<Supplier> SuppliersToRemove = new List<Supplier>();

                //find the suppliers that should no longer be selected because the
                //user removed them
                //remember, SelectedSuppliers = the list from the HTTP request (listbox)
                foreach (Supplier supplier in dbProduct.Suppliers)
                {
                    //see if the new list contains the supplier id from the old list
                    if (SelectedSuppliers.Contains(supplier.SupplierID) == false)//this supplier is not on the new list
                    {
                        SuppliersToRemove.Add(supplier);
                    }
                }

                //remove the suppliers you found in the list above
                //this has to be 2 separate steps because you can't iterate (loop)
                //over a list that you are removing things from
                foreach (Supplier supplier in SuppliersToRemove)
                {
                    //remove this product supplier from the product's list of suppliers
                    dbProduct.Suppliers.Remove(supplier);
                    _context.SaveChanges();
                }

                //add the suppliers that aren't already there
                foreach (int supplierID in SelectedSuppliers)
                {
                    if (dbProduct.Suppliers.Any(d => d.SupplierID == supplierID) == false)//this supplier is NOT already associated with this product
                    {
                        //Find the associated supplier in the database
                        Supplier dbSupplier = _context.Suppliers.Find(supplierID);

                        //Add the supplier to the product's list of suppliers
                        dbProduct.Suppliers.Add(dbSupplier);
                        _context.SaveChanges();
                    }
                }

                //update the product's scalar properties
                dbProduct.ProductID = product.ProductID;
                dbProduct.Name = product.Name;
                dbProduct.Description = product.Description;
                dbProduct.Price = product.Price;
                dbProduct.ProductType = product.ProductType;

                //save the changes
                _context.Products.Update(dbProduct);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                return View("Error", new string[] { "There was an error editing this product.", ex.Message });
            }

            //if code gets this far, everything is okay
            //send the user back to the page with all the products
            return RedirectToAction(nameof(Index));
        }


        private MultiSelectList GetAllSuppliers()
        {
            //Create a new list of suppliers and get the list of the suppliers
            //from the database
            List<Supplier> allSuppliers = _context.Suppliers.ToList();

            //use the MultiSelectList constructor method to get a new MultiSelectList
            MultiSelectList mslAllSuppliers = new MultiSelectList(allSuppliers.OrderBy(s => s.SupplierName), "SupplierID", "SupplierName");

            //return the MultiSelectList
            return mslAllSuppliers;



        }

        private MultiSelectList GetAllSuppliers(Product product)
        {
            //Create a new list of Suppliers and get the list of the Suppliers
            //from the database
            List<Supplier> allSuppliers = _context.Suppliers.ToList();

            //loop through the list of product Suppliers to find a list of Supplier ids
            //create a list to store the Supplier ids
            List<Int32> selectedSupplierIDs = new List<Int32>();

            //Loop through the list to find the SupplierIDs
            foreach (Supplier associatedSupplier in product.Suppliers)
            {
                selectedSupplierIDs.Add(associatedSupplier.SupplierID);
            }

            //use the MultiSelectList constructor method to get a new MultiSelectList
            MultiSelectList mslAllSuppliers = new MultiSelectList(allSuppliers.OrderBy(d => d.SupplierName), "SupplierID", "SupplierName", selectedSupplierIDs);

            //return the MultiSelectList
            return mslAllSuppliers;
        }
    }
}
