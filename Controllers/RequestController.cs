using RMC_Donation.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMC_Donation.Controllers
{
    public class RequestController : Controller
    {
        public ActionResult RequestItem(int itemId, int recId)
        {
            using (var db = new rmcDonationRequestItems())
            {
                var notificationEntity = new rmcDonateNotificationEntity();
                var notification = new notification();

                int senderid = (int) Session["user_id"];

                var findUser = new rmcdonateEntities().users.SingleOrDefault(u => u.id == senderid);
                var findItem = new rmcdonateItemsEntity().items.SingleOrDefault(i => i.id == itemId);

                var existingRequest = db.requestitems
                .SingleOrDefault(r => r.sender_id == senderid && r.receiver_id == recId && r.item_id == itemId);

                    if (existingRequest != null)
                    {
                        existingRequest.status = 1;
                    }
                    else
                    {
                        var newRequest = new requestitem
                        {
                            item_id = itemId,
                            receiver_id = recId,
                            sender_id = senderid,
                            approved_status = 2,
                            createdat = DateTime.Now,
                            updatedat = DateTime.Now,
                            status = 1,
                        };

                        db.requestitems.Add(newRequest);
                    }
                    
                    //for notification
                    notification.details = findUser.fullname + " has requested for item " + findItem.name + " for category " + findItem.catagory;
                    notification.status = 1;
                    notification.user_id = recId;
                    notification.createdat = DateTime.Now;
                    notificationEntity.notifications.Add(notification);
                    notificationEntity.SaveChanges();

                    db.SaveChanges();
            }
            return RedirectToAction("Index","Home");
        }

        public ActionResult Index()
        {
            var dbRequests = new rmcDonationRequestItems();
            var dbUser = new rmcdonateEntities();
            var dbItem = new rmcdonateItemsEntity();

            int senderId = (int)Session["user_id"];

            var myRequests = dbRequests.requestitems
                .Where(requestitem => requestitem.sender_id == senderId)
                .ToList();

            var userEntities = dbUser.users.ToList();
            var itemEntities = dbItem.items.ToList();

            // Combine data into the view model using joins
            var combinedData = myRequests
                .Join(
                    userEntities,
                    requestitem => requestitem.receiver_id,
                    user => user.id,
                    (requestitem, user) => new { RequestItem = requestitem, User = user })
                .Join(
                    itemEntities,
                    requestUser => requestUser.RequestItem.item_id,
                    item => item.id,
                    (requestUser, item) => new RequestItemWithUserAndItemViewModel
                    {
                        RequestItem = requestUser.RequestItem,
                        User = requestUser.User,
                        Item = item
                    })
                .ToList();

            return View(combinedData);
        }

        public ActionResult ItemRequests()
        {
            var dbRequests = new rmcDonationRequestItems();
            var dbUser = new rmcdonateEntities();
            var dbItem = new rmcdonateItemsEntity();

            int recId = (int)Session["user_id"];

            var myRequests = dbRequests.requestitems
                .Where(requestitem => requestitem.receiver_id == recId).Where(requestitem => requestitem.status != 0)
                .ToList();

            var userEntities = dbUser.users.ToList();
            var itemEntities = dbItem.items.ToList();

            // Combine data into the view model using joins
            var combinedData = myRequests
                .Join(
                    userEntities,
                    requestitem => requestitem.sender_id,
                    user => user.id,
                    (requestitem, user) => new { RequestItem = requestitem, User = user })
                .Join(
                    itemEntities,
                    requestUser => requestUser.RequestItem.item_id,
                    item => item.id,
                    (requestUser, item) => new RequestItemWithUserAndItemViewModel
                    {
                        RequestItem = requestUser.RequestItem,
                        User = requestUser.User,
                        Item = item
                    })
                .ToList();

            return View(combinedData);
        }

        [HttpPost]
        public ActionResult UpdateRequestStatus(int id, bool isChecked)
        {
            using (var db = new rmcDonationRequestItems())
            {
                var req = db.requestitems.Find(id);

                if (req != null)
                {
                    req.status = isChecked ? 1 : 0;
                    req.updatedat = DateTime.Now;
                    db.SaveChanges();
                }
            }
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult UpdateRequestStatusReceiverAfterDesicion(int reqId, bool isChecked, int item_id, int rec_id)
        {
            var notificationEntity = new rmcDonateNotificationEntity();
            var notification = new notification();

            var sender_id = (int)Session["user_id"];
            var findUser = new rmcdonateEntities().users.SingleOrDefault(u => u.id == sender_id);
            var findItem = new rmcdonateItemsEntity().items.SingleOrDefault(i => i.id == item_id);

            using (var db = new rmcDonationRequestItems())
            {
                var req = db.requestitems.Find(reqId);

                if (req != null)
                {
                    req.approved_status = isChecked ? 1 : 0;
                    req.updatedat = DateTime.Now;
                    db.SaveChanges();

                    if(isChecked)
                    {
                        notification.details = "Your Request for item " + findItem.name + " for category " + findItem.catagory + " is Accepted by "+findUser.fullname;
                    }
                    else
                    {
                        notification.details = "Your Request for item " + findItem.name + " for category " + findItem.catagory + " is Rejected by "+findUser.fullname;
                    }

                    notification.status = 1;
                    notification.user_id = rec_id;
                    notification.createdat = DateTime.Now;
                    notificationEntity.notifications.Add(notification);
                    notificationEntity.SaveChanges();
                }
            }
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult UpdateRequestStatusReceiver(int reqId, int newStatus, int item_id, int rec_id)
        {
            var notificationEntity = new rmcDonateNotificationEntity();
            var notification = new notification();

            var sender_id = (int)Session["user_id"];
            var findUser = new rmcdonateEntities().users.SingleOrDefault(u => u.id == sender_id);
            var findItem = new rmcdonateItemsEntity().items.SingleOrDefault(i => i.id == item_id);

            using (var db = new rmcDonationRequestItems())
            {
                var req = db.requestitems.Find(reqId);

                if (req != null)
                {
                    req.approved_status = newStatus;
                    req.updatedat = DateTime.Now;
                    db.SaveChanges();

                    if (newStatus == 1)
                    {
                        notification.details = "Your Request for item " + findItem.name + " for category " + findItem.catagory + " is Accepted by " + findUser.fullname;
                    }
                    else
                    {
                        notification.details = "Your Request for item " + findItem.name + " for category " + findItem.catagory + " is Rejected by " + findUser.fullname;
                    }

                    notification.status = 1;
                    notification.user_id = rec_id;
                    notification.createdat = DateTime.Now;
                    notificationEntity.notifications.Add(notification);
                    notificationEntity.SaveChanges();
                }
            }
            return Json(new {newStatus});
        }
    }
}