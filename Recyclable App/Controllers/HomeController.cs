using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Recyclable_App.Models;
using Recyclable_App.Services1;
using System;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.NetworkInformation;
using Recyclable_App.Models.Service1;

namespace Recyclable_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;

        public HomeController(ILogger<HomeController> logger, AppDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var recyclableTypes = _context.RecyclableTypes.ToList();
            var recyclableItems = _context.RecyclableItems.ToList();

            var viewModel = new IndexViewModel
            {
                RecyclableTypes = recyclableTypes,
                RecyclableItems = recyclableItems
            };

            return View(viewModel);
        }
      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        #region Recyclable type
        [HttpPost]
        public IActionResult ProcessForm(string type, int rate, decimal minKg, decimal maxKg)
        {
            try
            {
                // Check if any input field is empty or invalid
                if (string.IsNullOrEmpty(type))
                {
                    TempData["ErrorMessage"] = "Type is required.";
                    return RedirectToAction("Index");
                }

                if (rate <= 0)
                {
                    TempData["ErrorMessage"] = "Rate must be greater than zero.";
                    return RedirectToAction("Index");
                }

                if (minKg <= 0)
                {
                    TempData["ErrorMessage"] = "Minimum Kg must be greater than zero.";
                    return RedirectToAction("Index");
                }

                if (maxKg <= 0)
                {
                    TempData["ErrorMessage"] = "Maximum Kg must be greater than zero.";
                    return RedirectToAction("Index");
                }

                // Check if the type already exists
                var existingType = _context.RecyclableTypes.FirstOrDefault(rt => rt.Type == type);
                if (existingType != null)
                {
                    TempData["Message"] = "Type already exists.";
                    return RedirectToAction("Index");
                }

                var recyclableType = new RecyclableTypes
                {
                    Type = type,
                    Rate = rate,
                    MinKg = minKg,
                    MaxKg = maxKg
                };

                _context.RecyclableTypes.Add(recyclableType);
                _context.SaveChanges();

                TempData["SaveMessage"] = "Saved successfully!";
                return RedirectToAction("Index");
            }
            catch (FormatException ex)

            {
                Debug.WriteLine(ex.Message);
                // Log the exception (not shown here)
                return BadRequest("Invalid input format for rate, minKg, or maxKg.");
            }
        }
        public IActionResult View(int id)
        {
            try
            {
                // Find the recyclable type by ID
                var recyclableType = _context.RecyclableTypes.Find(id);

                // Check if the recyclable type exists
                if (recyclableType == null)
                {
                    // Redirect to the Index action with an error message if the type does not exist
                    TempData["ErrorMessage"] = "Recyclable type not found.";
                    return RedirectToAction("Index");
                }

                // Return the recyclable type to the View
                return View(recyclableType);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // Log the exception (not shown here)
                return BadRequest("An error occurred while processing your request.");
            }
        }

        // GET: Home/UpdateType/id
        public IActionResult UpdateType(int id)
        {
            try
            {
                var recyclableType = _context.RecyclableTypes.Find(id);

                if (recyclableType == null)
                {
                    TempData["ErrorMessage"] = "Recyclable type not found.";
                    return RedirectToAction("Index");
                }

                return View(recyclableType);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while updating recyclable type.");
                return BadRequest("An error occurred while processing your request.");
            }
        }

        // POST: Home/Updatetype
        [HttpPost]
        public IActionResult UpdateType(RecyclableTypes updatedType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingType = _context.RecyclableTypes.FirstOrDefault(rt => rt.Id == updatedType.Id);

                    if (existingType == null)
                    {
                        TempData["ErrorMessage"] = "Recyclable type not found.";
                        return RedirectToAction("Index");
                    }

                    // Update the existing type
                    existingType.Type = updatedType.Type;
                    existingType.Rate = updatedType.Rate;
                    existingType.MinKg = updatedType.MinKg;
                    existingType.MaxKg = updatedType.MaxKg;

                    _context.SaveChanges();

                    TempData["UpdateMessage"] = "Successfully Updated!.";
                    return RedirectToAction("Index");
                }

                // If ModelState is not valid, return to the update form
                return View(updatedType);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while updating recyclable type.");
                return BadRequest("An error occurred while processing your request.");
            }
        }

        // GET: Home/DeleteTypes/id
        public IActionResult DeleteTypes(int id)
        {
            try
            {
                var recyclableType = _context.RecyclableTypes.Find(id);

                if (recyclableType == null)
                {
                    TempData["ErrorMessage"] = "Recyclable type not found.";
                    return RedirectToAction("Index");
                }

                return View(recyclableType);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToAction("Index");
            }
        }

        // POST: Home/DeleteTypes
        [HttpPost, ActionName("DeleteTypes")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var recyclableType = _context.RecyclableTypes.Find(id);

                if (recyclableType == null)
                {
                    TempData["ErrorMessage"] = "Recyclable type not found.";
                    return RedirectToAction("Index");
                }

                var recyclableItems = _context.RecyclableItems.Where(item => item.RecyclableTypeId == id).ToList();

                if (recyclableItems != null && recyclableItems.Any())
                {
                    _context.RecyclableItems.RemoveRange(recyclableItems);
                }

                _context.RecyclableTypes.Remove(recyclableType);

                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception
                Debug.WriteLine(ex.Message, "An error occurred while processing your request.");
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return RedirectToAction("Index");
            }
        }
        #endregion


        #region Recyclable Item
        [HttpPost]
        public IActionResult ItemForm(int RecyclableTypeId, string Description, decimal Weight, decimal ComputedRate)
        {
            try
            {
                // Validate RecyclableTypeId
                if (RecyclableTypeId <= 0)
                {
                    TempData["Type"] = "Type is required.";
                    return RedirectToAction("Index");
                }

                // Validate Description
                if (string.IsNullOrEmpty(Description))
                {
                    TempData["Description"] = "Description is required.";
                    return RedirectToAction("Index");
                }

                // Validate Weight
                if (Weight <= 0)
                {
                    TempData["Weight"] = "Weight must be greater than 0.";
                    return RedirectToAction("Index");
                }

                // Validate ComputedRate
                if (ComputedRate <= 0)
                {
                    TempData["ComputedRate"] = "ComputedRate must be greater than 0.";
                    return RedirectToAction("Index");
                }

                // Create a new RecyclableItems object
                var recyclableItems = new RecyclableItems
                {
                    RecyclableTypeId = RecyclableTypeId,
                    Description = Description,
                    Weight = Weight,
                    ComputedRate = ComputedRate
                };

                // Add the new item to the database
                _context.RecyclableItems.Add(recyclableItems);
                _context.SaveChanges();

                // Set success message and redirect to Index
                TempData["SaveMessage1"] = "Save Successfully!.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception (logging not shown here)
                Debug.WriteLine(ex.Message);
                return BadRequest("An error occurred while processing your request.");
            }
        }

        // GET: Home/UpdateItem/id
        public IActionResult UpdateItem(int id)
        {
            try
            {
                var recyclableItems = _context.RecyclableItems.Find(id);

                if (recyclableItems == null)
                {
                    TempData["ErrorMessage"] = "Recyclable type not found.";
                    return RedirectToAction("Index");
                }

                return View(recyclableItems);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while updating recyclable type.");
                return BadRequest("An error occurred while processing your request.");
            }
        }

        // POST: Home/UpdateItem
        [HttpPost]
        public IActionResult UpdateItem(RecyclableItems updatedItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingType = _context.RecyclableItems.FirstOrDefault(rt => rt.Id == updatedItem.Id);

                    if (existingType == null)
                    {
                        TempData["ErrorMessage"] = "Recyclable type not found.";
                        return RedirectToAction("Index");
                    }

                    // Update the existing type
                    existingType.RecyclableTypeId = updatedItem.RecyclableTypeId;
                    existingType.Description = updatedItem.Description;
                    existingType.Weight = updatedItem.Weight;
                    existingType.ComputedRate = updatedItem.ComputedRate;

                    _context.SaveChanges();

                    TempData["UpdateMessage1"] = "Successfully Updated!.";
                    return RedirectToAction("Index");
                }

                // If ModelState is not valid, return to the update form
                return View(updatedItem);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while updating recyclable type.");
                return BadRequest("An error occurred while processing your request.");
            }
        }

        // GET: Home/DeleteType/id
        public IActionResult DeleteItem(int id)
        {
            try
            {
                var recyclableItem = _context.RecyclableItems.Find(id);

                if (recyclableItem == null)
                {
                    TempData["ErrorMessage"] = "Recyclable type not found.";
                    return RedirectToAction("Index");
                }

                return View(recyclableItem);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while deleting recyclable type.");
                return BadRequest("An error occurred while processing your request.");
            }
        }

        // POST: Home/DeleteType
        [HttpPost, ActionName("DeleteItem")]
        public IActionResult DeleteItemConfirmed(int id)
        {
            try
            {
                var recyclableItem = _context.RecyclableItems.Find(id);

                if (recyclableItem == null)
                {
                    TempData["ErrorMessage"] = "Recyclable type not found.";
                    return RedirectToAction("Index");
                }

                _context.RecyclableItems.Remove(recyclableItem);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while deleting recyclable type.");
                return BadRequest("An error occurred while processing your request.");
            }
        }
        #endregion

    }

    internal class IndexViewModel
    {
        public List<RecyclableTypes> RecyclableTypes { get; set; }
        public List<RecyclableItems> RecyclableItems { get; set; }
    }
}
