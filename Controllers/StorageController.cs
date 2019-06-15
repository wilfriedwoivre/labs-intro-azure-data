using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Soat.Masterclass.Labs.Models.Storage;
using Soat.Masterclass.Labs.Repositories;

namespace Soat.Masterclass.Labs.Controllers
{
    public class StorageController : Controller
    {
        private readonly IStorageBlobHelper storage;
        public StorageController(IStorageBlobHelper storage)
        {
            this.storage = storage;
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            var items = await storage.ListBlobAsync();
            return View(items);
        }

#pragma warning disable 1998
        [ActionName("Create")]
        public async Task<IActionResult> CreateAsync()
        {
            return View();
        }
#pragma warning restore 1998

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("Name,Content")] BlobItem item)
        {
            if (ModelState.IsValid)
            {
                await storage.CreateBlobAsync(item.Name, item.Content);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("Name,Content")] BlobItem item)
        {
            if (ModelState.IsValid)
            {
                await storage.UpdateBlobAsync(item.Name, item.Content);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string fileName)
        {
            if (fileName == null)
            {
                return BadRequest();
            }

            BlobItem item = await storage.GetBlobContentAsync(fileName);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string fileName)
        {
            if (fileName == null)
            {
                return BadRequest();
            }

            BlobItem item = await storage.GetBlobContentAsync(fileName);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("Name")] string name)
        {
            await storage.DeleteBlobAsync(name);
            return RedirectToAction("Index");
        }
    }
}