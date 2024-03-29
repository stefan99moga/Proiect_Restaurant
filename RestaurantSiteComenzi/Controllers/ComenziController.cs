﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSiteComenzi.Models;

namespace RestaurantSiteComenzi.Controllers
{
    [Authorize(Roles = "Admin, Bucatar")]
    public class ComenziController : Controller
    {
        public Uri uri = new Uri("https://localhost:44305/api/");
        private readonly RestaurantContext _context;
        public ComenziController(RestaurantContext context)
        {
            _context = context;
        }
        public ViewResult Index(string sortOrder, int searchId)
        {
            //Sortare dupa stare comanda
            ViewData["IndecisaParam"] = String.IsNullOrEmpty(sortOrder) ? "indecisa" : "indecisa";
            ViewData["PrimitaParam"] = String.IsNullOrEmpty(sortOrder) ? "primita" : "primita";
            ViewData["ConfirmataParam"] = String.IsNullOrEmpty(sortOrder) ? "confirmata" : "confirmata";
            ViewData["RefuzataParam"] = String.IsNullOrEmpty(sortOrder) ? "refuzata" : "refuzata";
            ViewData["AnulataParam"] = String.IsNullOrEmpty(sortOrder) ? "anulata" : "anulata";
            ViewData["LivrataParam"] = String.IsNullOrEmpty(sortOrder) ? "livrata" : "livrata";

            using (var client = new HttpClient())
            {
                client.BaseAddress = uri;
                var url_string = "";


                if (searchId > 0)
                {
                    url_string = "Comenzi/" + searchId;
                }
                else
                {
                    url_string = "Comenzi?sortOrder=" + sortOrder;
                }

                var responseTask = client.GetAsync(url_string);
                responseTask.Wait();
                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    if (searchId > 0)
                    {
                        Comenzi[] temp_list = new Comenzi[1];
                        var  readTask1 = result.Content.ReadFromJsonAsync<Comenzi>();
                        readTask1.Wait();
                        temp_list[0] = readTask1.Result;
                        var comenzi = temp_list;
                        if (comenzi.Length == 0)
                        {
                            return View("EmptyOrders");
                        }
                        return View(comenzi.OrderByDescending(x => x.Data_Comanda));
                    }
                    else
                    {
                        var readTask = result.Content.ReadFromJsonAsync<Comenzi[]>();
                        readTask.Wait();
                        var comenzi = readTask.Result;

                        if (comenzi.Length == 0)
                        {
                            return View("EmptyOrders");
                        }

                        return View(comenzi.OrderByDescending(x => x.Data_Comanda));
                    }

                }
                else
                {
                    return View("EmptyOrders");
                }
            }
        }
        public ViewResult Edit(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = uri;
                var responseTask = client.GetAsync("Comenzi/" + id);
                responseTask.Wait();
                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadFromJsonAsync<Comenzi>();
                    readTask.Wait();

                    var comenzi = readTask.Result;

                    return View(comenzi);
                }
                else
                {
                    return View();
                }
            }

        }
        public ActionResult GetStareComanda()
        {
            var order_states = _context.Stare_Comanda.Select(a => new
            {
                a.Id,
                a.Nume
            });

            return Json(order_states.ToList());
        }
        public ViewResult Details(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = uri;
                var responseTask = client.GetAsync("Cart/GetComandaDetails?comandaId=" + id);
                responseTask.Wait();
                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadFromJsonAsync<Cos[]>();
                    readTask.Wait();

                    var comenzi = readTask.Result;

                    return View(comenzi);
                }
                else
                {
                    return View();
                }
            }
        }
    }
}
