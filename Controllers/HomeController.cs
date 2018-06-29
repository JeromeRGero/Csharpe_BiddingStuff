using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginRegDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LoginRegDemo.Controllers {
    public class HomeController : Controller {

        private UserContext _context;
        // private object userFactory;

        public HomeController (UserContext context) {
            _context = context;
        }


        // ***********************************************************************


        public IActionResult Index () {
            List<Prod> expired = _context.prods.Include(a => a.User).Where(p => p.EndDate < DateTime.Now).ToList();
            foreach(var expProd in expired)
            {
                if(expProd.BidderId != 0)
                {
                    expProd.User.Wallet += expProd.StartingBid;
                }
                else{
                    _context.prods.Remove(expProd);
                }
                _context.SaveChanges();
            }
            return View ();
        }


        [HttpPost]
        [Route ("RegisterUser")]
        public IActionResult RegisterUser (User MyUser, string ConfirmPassword) {
            System.Console.WriteLine ("WE HIT REGISTERED USER FUNCTION IN CONTROLLER");
            if(MyUser.Password != ConfirmPassword) {
                System.Console.WriteLine("\n****************Passwords dont match****************\n\n");
                ViewBag.PasswordError = $"{MyUser.FirstName} I know this must be hard for you, but lets try writing matching passwords this time.";
                return View ("Index");
            }

            if (ModelState.IsValid) {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                MyUser.Password = Hasher.HashPassword(MyUser, MyUser.Password);
                User ExistingUser = _context.users.SingleOrDefault (u => u.Email == MyUser.Email);
                if (ExistingUser != null) {
                    System.Console.WriteLine (" *************EMAIL ALREADY IN USE**********************");
                    ViewBag.AlreadyInUseEmail = true;
                    // ViewBag.AlreadyInUseEmail = $"{MyUser.Email} is already in the Data base, YOU FUCK!";
                    return View ("Index");
                    // Yo dude Have you ever watched Mike Tyson Mysteries? Its really good show.
                }
                else{
                    MyUser.Wallet = 1000.00f;
                    _context.Add (MyUser);
                    _context.SaveChanges ();
                    // grabs all reviews from database
                    User results = _context.users.SingleOrDefault(u => u.Email == MyUser.Email);
                    int userid = results.idUsers;
                    HttpContext.Session.SetInt32("logged_id", userid);
                    return RedirectToAction ("Success");
                }
            } else {
                System.Console.WriteLine ("There were errors adding user returned to index********************");
                return View ("Index");
            }

        }


        public IActionResult Success () 
        {
            if (HttpContext.Session.GetInt32("logged_id") == null) {
                return RedirectToAction("Index");
            }
            int curUserid = (int)HttpContext.Session.GetInt32("logged_id");
            User curUser = _context.users.SingleOrDefault(u => u.idUsers == curUserid);
            ViewBag.curMoney = curUser.Wallet;
            ViewBag.Username = curUser.FirstName;
            // Storing the current user (curUser) as an obj.
            ViewBag.curUserId = curUserid;
            // All Products
            List<Prod> AllProd = _context.prods.Include(u => u.User).OrderBy(uhi => uhi.EndDate).ToList();
            ViewBag.AllProd = AllProd;
            messwithstuff();
            return View ();
        }

        [Route("NewAuc")]
        public IActionResult NewAuc()
        {
            return View();
        }


        [Route("SubmitProduct")]
        public IActionResult SubmitProduct(Prod newAuc)
        {
            int id = (int)HttpContext.Session.GetInt32("logged_id");
            newAuc.PostersId = id;
            System.Console.WriteLine(newAuc.EndDate);
            if(ModelState.IsValid && (newAuc.StartingBid > 0) && (newAuc.EndDate > DateTime.Now))
            {
                System.Console.WriteLine("It would work I think!");
                _context.prods.Add(newAuc);
                _context.SaveChanges();
            }
            else
            {
                System.Console.WriteLine("################# Nope! ###################");
            }
            return RedirectToAction("Success");
        }

        
        [Route("Userpage/{uid}")]
        public IActionResult Userpage(int uid)
        {
            // System.Console.WriteLine("Help?");
            // User curUserLookingAt = _context.users.SingleOrDefault(u => u.idUsers == uid);
            // ViewBag.UserObj = curUserLookingAt;
            // List<Idea> curUserIdeas = _context.ideas.Where(i => i.UseridUsers == uid).ToList(); 
            // int? ic = curUserIdeas.Count;
            // if(ic != null){
            //     int ic2 = (int)ic;
            //     ViewBag.numOfPosts = ic2;
            // }else{
            //     ViewBag.numOfPosts = 0;
            // }
            // List<Users_Has_Idea> ul = _context.users_has_ideas.Where(ulid => ulid.Users_idUsers == uid).ToList();
            // int? uli = ul.Count;
            // if(uli != null){
            //     int uli2 = (int)uli;
            //     ViewBag.numOfLikes = uli2;
            // }else{
            //     ViewBag.numOfLikes = 0;
            // }
            return View("Userpage");
        }


        [HttpPost]
        [Route("LoginUser")]
        public IActionResult Login(string Email, string Password){
            
            var user = _context.users.SingleOrDefault(u => u.Email == Email);
            if(user != null && Password != null){
                var Hasher = new PasswordHasher<User>();
                if(0 != Hasher.VerifyHashedPassword(user, user.Password, Password)){
                    User results = _context.users.SingleOrDefault(u => u.Email == Email);
                    int userid = results.idUsers;
                    HttpContext.Session.SetInt32("logged_id", userid);
                    return RedirectToAction("Success");
                }
                else{
                    System.Console.WriteLine("*************Password doesnt match*************");
                    ViewBag.loginError = "Wrong password!";
                    return View("Index");
                }
            }
            else{
                System.Console.WriteLine("*************Email doesnt exist*************");
                ViewBag.loginError = "Email not registered";
                return View("Index");
            }
            
        }

        [HttpGet]
        [Route("PostsPage/{prodsid}")]
        public IActionResult Ideapage(int prodsid){
            // Idea ideaforpage = _context.ideas.SingleOrDefault(i => i.idIdeas == ideaID);
            // ViewBag.TheIdea = ideaforpage;
            // User poster = _context.users.SingleOrDefault(u => u.idUsers == ideaforpage.UseridUsers);
            // ViewBag.TheIdeaMaker = poster.Name;
            // List<Users_Has_Idea> LikeRecords = _context.users_has_ideas.Where(uhi => uhi.Ideas_idIdeas == ideaID).Include(u => u.User).ToList();
            // ViewBag.PeopleThatLikedThis = LikeRecords;
            return View("Ideapage");
        }




        [HttpPost]
        [Route("IdeaPost")]
        public IActionResult IdeaPost(Prod newIdea){
            // System.Console.WriteLine("You made it!");
            // int curUserid = (int)HttpContext.Session.GetInt32("logged_id");
            // User curUser = _context.users.SingleOrDefault(u => u.idUsers == curUserid);
            // newIdea.UseridUsers = curUserid;
            // if(ModelState.IsValid){
            //     System.Console.WriteLine("Its Valid");
            //     _context.ideas.Add(newIdea);
            //     _context.SaveChanges ();
            //     System.Console.WriteLine("Saved Changes");
            //     return RedirectToAction ("Success");
            // }
            // else{
            //     System.Console.WriteLine("Model State invalid?");
            //     return RedirectToAction("Success");
            // }
            return RedirectToAction("Success");
        }

        
        [Route("Logout")]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }


        [Route("Delete/{prodsid}")]
        public IActionResult DeleteIdea(int prodsid){
            System.Console.WriteLine("Hit Delete Route");
            int curUsrID = (int)HttpContext.Session.GetInt32("logged_id");
            Prod curProd = _context.prods.SingleOrDefault(i => (i.ProdId == prodsid && i.PostersId == curUsrID));
            if(curProd != null){
                // return Money to bidder
                System.Console.WriteLine("Made it!");
                User bidder = _context.users.SingleOrDefault(u => u.idUsers == curProd.BidderId);
                bidder.Wallet += curProd.StartingBid;
                _context.SaveChanges();
                System.Console.WriteLine("REMOVING ##################\n###################");
                _context.prods.Remove(curProd);
                _context.SaveChanges();    
            }           
            System.Console.WriteLine("#####################\nFinished!");
            return RedirectToAction("Success");
        }

        [Route("PostsPage/{pid}")]
        public IActionResult PostsPage(int pid)
        {
            Prod curProd = _context.prods.Include(p => p.User).SingleOrDefault(p => p.ProdId == pid);
            ViewBag.CreatedBy = curProd.User.FirstName;
            ViewBag.Description = curProd.Description;
            ViewBag.ProductName = curProd.ProductName;
            ViewBag.StartingBid = curProd.StartingBid;
            ViewBag.pid = curProd.ProdId;
            if(curProd.BidderId != 0)
            {
                ViewBag.BiddersName = _context.users.SingleOrDefault(u => u.idUsers == curProd.BidderId).FirstName;
                return View();
            }
            ViewBag.BiddersName = "No one yet!";
            return View();
        }


        [HttpPost]
        [Route("newbid/{productID}")]
        public IActionResult newbid(int productID, int bidding)
        {
            int curUsrID = (int)HttpContext.Session.GetInt32("logged_id");
            User curUser = _context.users.SingleOrDefault(u => u.idUsers == curUsrID);
            Prod curProd = _context.prods.SingleOrDefault(p => p.ProdId == productID);
            System.Console.WriteLine(bidding);
            System.Console.WriteLine(curUser.Wallet);
            System.Console.WriteLine(curProd.StartingBid);
            System.Console.WriteLine("##################################################");
            if(curUser.Wallet < bidding || curProd.StartingBid >= bidding)
            {
                return RedirectToAction("Logout");
            }
            if(curProd.BidderId != 0)
            {
                User cheapskate = _context.users.SingleOrDefault(u => u.idUsers == curProd.BidderId);
                cheapskate.Wallet += curProd.StartingBid;
            }
            curUser.Wallet -= bidding;
            curProd.BidderId = curUsrID;
            curProd.BidderName = curUser.FirstName;
            curProd.StartingBid = bidding;
            _context.SaveChanges();
            return RedirectToAction("Success");
        }

        public IActionResult Error () {
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void messwithstuff()
        {
            List<Prod> expired = _context.prods.Include(a => a.User).Where(p => p.EndDate < DateTime.Now).ToList();
            foreach(var expProd in expired)
            {
                if(expProd.BidderId != 0)
                {
                    expProd.User.Wallet += expProd.StartingBid;
                }
                else{
                    _context.prods.Remove(expProd);
                }
                _context.SaveChanges();
            }
        }
    }
}