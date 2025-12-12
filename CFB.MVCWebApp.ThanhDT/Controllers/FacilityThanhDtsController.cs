using CFB.Entities.ThanhDT.Models;
using CFB.Repositories.ThanhDT.DBContext;
using CFB.Services.ThanhDT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFB.MVCWebApp.ThanhDT.Controllers
{
    public class FacilityThanhDtsController : Controller
    {
        //private readonly FA25_PRN222_3W_G2_CampusFacilityBookingContext _context;

        private readonly FacilityThanhDtService _facilityThanhDtService;
        private readonly FacilityTypeThanhDtService _facilityTypeThanhDtService;
        private readonly CampusThanhDtService _campusThanhDtService;

        //public FacilityThanhDtsController(FA25_PRN222_3W_G2_CampusFacilityBookingContext context)
        public FacilityThanhDtsController(FacilityThanhDtService facilityThanhDtService, FacilityTypeThanhDtService facilityTypeThanhDtService, CampusThanhDtService campusThanhDtService)
        {
            _facilityThanhDtService = facilityThanhDtService;
            _facilityTypeThanhDtService = facilityTypeThanhDtService;
            _campusThanhDtService = campusThanhDtService;
        }

        // GET: FacilityThanhDts
        //public async Task<IActionResult> Index()
        //{
        //    //var fA25_PRN222_3W_G2_CampusFacilityBookingContext = _context.FacilityThanhDts.Include(f => f.CampusThanhDt).Include(f => f.FacilityTypeThanhDt);
        //    //return View(await fA25_PRN222_3W_G2_CampusFacilityBookingContext.ToListAsync());

        //    var items = await _facilityThanhDtService.GetAllAsync();
        //    return View(items);
        //}

        // GET: FacilityThanhDts
        public async Task<IActionResult> Index(string facilityName, int capacity, string typeName, int page = 1)
        {
            int pageSize = 5; // 5 items per page

            var allItems = await _facilityThanhDtService.SearchAsync(facilityName, capacity, typeName);

            // Calculate pagination
            var totalItems = allItems.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Ensure page is within valid range
            page = Math.Max(1, Math.Min(page, totalPages));

            // Get items for current page
            var items = allItems
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass pagination info to view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;

            return View(items);
        }

        // GET: FacilityThanhDts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var facilityThanhDt = await _context.FacilityThanhDts
            //    .Include(f => f.CampusThanhDt)
            //    .Include(f => f.FacilityTypeThanhDt)
            //    .FirstOrDefaultAsync(m => m.FacilityThanhDtid == id);

            var facilityThanhDt = await _facilityThanhDtService.GetByIdAsync(id.Value);

            if (facilityThanhDt == null)
            {
                return NotFound();
            }

            return View(facilityThanhDt);
        }

        // GET: FacilityThanhDts/Create
        // lấy dữ liệu đổ vô form tạo mới
        public async Task<IActionResult> Create()
        {
            //ViewData["CampusThanhDtid"] = new SelectList(_context.CampusThanhDts, "CampusThanhDtid", "Name");
            //ViewData["FacilityTypeThanhDtid"] = new SelectList(_context.FacilityTypeThanhDts, "FacilityTypeThanhDtid", "TypeName");
            //return View();

            var facilityType = await _facilityTypeThanhDtService.GetAllAsync();
            ViewData["FacilityTypeThanhDtid"] = new SelectList(facilityType, "FacilityTypeThanhDtid", "TypeName");

            var campus = await _campusThanhDtService.GetAllAsync();

            ViewData["CampusThanhDtid"] = new SelectList(campus, "CampusThanhDtid", "Name");

            /// ThanhDT | Set default values for the new FacilityThanhDt
            var item = new FacilityThanhDt()
            {
                CreatedAt = DateTime.Now,
                IsAvailable = true
            };
            return View(item);

        }

        // POST: FacilityThanhDts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FacilityThanhDt facilityThanhDt)
        {
            if (ModelState.IsValid)
            {
                var result = await _facilityThanhDtService.CreateAsync(facilityThanhDt);
                if (result > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            var facilityType = await _facilityTypeThanhDtService.GetAllAsync();
            ViewData["FacilityTypeThanhDtid"] = new SelectList(facilityType, "FacilityTypeThanhDtid", "TypeName", facilityThanhDt.FacilityThanhDtid);

            var campus = await _campusThanhDtService.GetAllAsync();

            ViewData["CampusThanhDtid"] = new SelectList(campus, "CampusThanhDtid", "Name", facilityThanhDt.CampusThanhDtid);
            return View(facilityThanhDt);
        }


        // GET: FacilityThanhDts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facilityThanhDt = await _facilityThanhDtService.GetByIdAsync(id.Value);
            if (facilityThanhDt == null)
            {
                return NotFound();
            }

            var facilityType = await _facilityTypeThanhDtService.GetAllAsync();
            ViewData["FacilityTypeThanhDtid"] = new SelectList(facilityType, "FacilityTypeThanhDtid", "TypeName", facilityThanhDt.FacilityThanhDtid);

            var campus = await _campusThanhDtService.GetAllAsync();

            ViewData["CampusThanhDtid"] = new SelectList(campus, "CampusThanhDtid", "Name", facilityThanhDt.CampusThanhDtid);

            return View(facilityThanhDt);
        }


        // POST: FacilityThanhDts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FacilityThanhDtid,FacilityName,FacilityTypeThanhDtid,CampusThanhDtid,Capacity,LocationDetail,Description,IsAvailable,CreatedAt,UpdatedAt")] FacilityThanhDt facilityThanhDt)
        {
            if (id != facilityThanhDt.FacilityThanhDtid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _facilityThanhDtService.UpdateAsync(facilityThanhDt);
                    if (result > 0)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            var facilityType = await _facilityTypeThanhDtService.GetAllAsync();
            ViewData["FacilityTypeThanhDtid"] = new SelectList(facilityType, "FacilityTypeThanhDtid", "TypeName", facilityThanhDt.FacilityThanhDtid);

            var campus = await _campusThanhDtService.GetAllAsync();

            ViewData["CampusThanhDtid"] = new SelectList(campus, "CampusThanhDtid", "Name", facilityThanhDt.CampusThanhDtid);

            return View(facilityThanhDt);
        }


        // GET: FacilityThanhDts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var facilityThanhDt = await _facilityThanhDtService.GetByIdAsync(id.Value);
            if (facilityThanhDt == null)
            {
                return NotFound();
            }

            return View(facilityThanhDt);
        }
        
        // POST: FacilityThanhDts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facilityThanhDt = await _facilityThanhDtService.DeleteAsyc(id);
            if (facilityThanhDt)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Xoá thất bại. Vui lòng thử lại.");
            return RedirectToAction(nameof(Delete), new { id });
        }
        //private bool FacilityThanhDtExists(int id)
        //{
        //    return _context.FacilityThanhDts.Any(e => e.FacilityThanhDtid == id);
        //}
    }
}
