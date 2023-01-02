using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Wang_Gloria_HW5.DAL;
using Wang_Gloria_HW5.Models;

namespace Wang_Gloria_HW5.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly AppDbContext _context;

        public OrderDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public IActionResult Index(int? orderID)
        {
            if (orderID == null)
            {
                return View("Error", new String[] { "Please specify a order to view!" });
            }

            //limit the list to only the order details that belong to this order
            List<OrderDetail> rds = _context.OrderDetails
                                          .Include(rd => rd.Product)
                                          .Where(rd => rd.Order.OrderID == orderID)
                                          .ToList();

            return View(rds);
        }

        // GET: OrderDetails/Create
        public IActionResult Create(int orderID)
        {
            //create a new instance of the OrderDetail class
            OrderDetail rd = new OrderDetail();

            //find the order that should be associated with this order
            Order dbOrder = _context.Orders.Find(orderID);

            //set the new order detail's order equal to the order you just found
            rd.Order = dbOrder;

            //populate the ViewBag with a list of existing products
            ViewBag.AllProducts = GetAllProducts();

            //pass the newly created order detail to the view
            return View(rd);
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Order, OrderDetailID,Quantity")] OrderDetail orderDetail, int SelectedProduct)
        {
            //if user has not entered all fields, send them back to try again
            if (ModelState.IsValid == false)
            {
                ViewBag.AllProducts = GetAllProducts();
                return View(orderDetail);
            }

            //find the product to be associated with this order
            Product dbProduct = _context.Products.Find(SelectedProduct);

            //set the order detail's product to be equal to the one we just found
            orderDetail.Product = dbProduct;

            //find the order on the database that has the correct order id
            //unfortunately, the HTTP request will not contain the entire order object, 
            //just the order id, so we have to find the actual object in the database
            Order dbOrder = _context.Orders.Find(orderDetail.Order.OrderID);

            //set the order on the order detail equal to the order that we just found
            orderDetail.Order = dbOrder;

            //set the order detail's price equal to the product price
            //this will allow us to to store the price that the user paid
            orderDetail.ProductPrice = dbProduct.Price;

            //calculate the extended price for the order detail
            orderDetail.ExtendendedPrice = orderDetail.Quantity * orderDetail.ProductPrice;

            //add the order detail to the database
            _context.Add(orderDetail);
            await _context.SaveChangesAsync();

            //send the user to the details page for this order
            return RedirectToAction("Details", "Orders", new { id = orderDetail.Order.OrderID });
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //user did not specify a order detail to edit
            if (id == null)
            {
                return View("Error", new String[] { "Please specify a order detail to edit!" });
            }

            //find the order detail
            OrderDetail orderDetail = await _context.OrderDetails
                                                   .Include(rd => rd.Product)
                                                   .Include(rd => rd.Order)
                                                   .FirstOrDefaultAsync(rd => rd.OrderDetailID == id);
            if (orderDetail == null)
            {
                return View("Error", new String[] { "This order detail was not found" });
            }
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderDetailID,Quantity")] OrderDetail orderDetail)
        {
            //this is a security check to make sure they are editing the correct record
            if (id != orderDetail.OrderDetailID)
            {
                return View("Error", new String[] { "There was a problem editing this record. Try again!" });
            }

            //information is not valid, try again
            if (ModelState.IsValid == false)
            {
                return View(orderDetail);
            }

            //create a new order detail
            OrderDetail dbRD;
            //if code gets this far, update the record
            try
            {
                //find the existing order detail in the database
                //include both order and product
                dbRD = _context.OrderDetails
                      .Include(rd => rd.Product)
                      .Include(rd => rd.Order)
                      .FirstOrDefault(rd => rd.OrderDetailID == orderDetail.OrderDetailID);

                //update the scalar properties
                dbRD.Quantity = orderDetail.Quantity;
                dbRD.ProductPrice = dbRD.Product.Price;
                dbRD.ExtendendedPrice = dbRD.Quantity * dbRD.ProductPrice;

                //save changes
                _context.Update(dbRD);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return View("Error", new String[] { "There was a problem editing this record", ex.Message });
            }

            //if code gets this far, go back to the order details index page
            return RedirectToAction("Details", "Orders", new { id = dbRD.Order.OrderID });
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //user did not specify a order detail to delete
            if (id == null)
            {
                return View("Error", new String[] { "Please specify a order detail to delete!" });
            }

            //find the order detail in the database
            OrderDetail orderDetail = await _context.OrderDetails
                                                    .Include(r => r.Order)
                                                   .FirstOrDefaultAsync(m => m.OrderDetailID == id);

            //order detail was not found in the database
            if (orderDetail == null)
            {
                return View("Error", new String[] { "This order detail was not in the database!" });
            }

            //send the user to the delete confirmation page
            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //find the order detail to delete
            OrderDetail orderDetail = await _context.OrderDetails
                                                   .Include(r => r.Order)
                                                   .FirstOrDefaultAsync(r => r.OrderDetailID == id);

            //delete the order detail
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();

            //return the user to the order/details page
            return RedirectToAction("Details", "Orders", new { id = orderDetail.Order.OrderID });
        }

        private SelectList GetAllProducts()
        {
            //create a list for all the products
            List<Product> allProducts = _context.Products.ToList();

            //the user MUST select a product, so you don't need a dummy option for no product

            //use the constructor on select list to create a new select list with the options
            SelectList slAllProducts = new SelectList(allProducts, nameof(Product.ProductID), nameof(Product.Name));

            return slAllProducts;
        }
    }
}
