using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using System.Web.Services.Description;

using LeoAirlines.Models;

namespace LeoAirlines.Controllers
{
    public class airportinfoesController : Controller
    {
        public AirportDBEntities db = new AirportDBEntities();



        // GET: airportinfoes
        public ActionResult Index()
        {

            return View();
        }




        public ActionResult Create()
        {
            var cityList = db.cityinfoes.ToList();
            ViewBag.CityList1 = new SelectList(cityList, "CITY", "CITY");

            ViewBag.CityList2 = new SelectList(cityList, "CITY", "CITY");
            return View();
        }




        [HttpPost]
        public ActionResult Create(FormCollection form)
        {

            var cityList = db.cityinfoes.ToList();



            string From = form["CityList1"].ToString();
            cityinfo city1 = cityList.Find(m => m.CITY == From);
            var startlocation = new Location(city1.LAT, city1.LONG);
            string To = form["CityList2"].ToString();
            cityinfo city2 = cityList.Find(m => m.CITY == To);



            var destinationlocation = new Location(city2.LAT, city2.LONG);
            if (From == To)
            {
                TempData["Error"] = "Source and destination cannot be same";
                return RedirectToAction("Create");
            }
            else
            {
                var airportsInRange = new List<airportinfo>(); /// airports between cities
                var airinrange = new List<airinfo>();
                var airports = db.airportinfoes.ToList();



                //this is comment




                var maxDistance = HaversineDistance(startlocation, destinationlocation) + 50;
                foreach (var airport in airports)
                {



                    var airportLocation = new Location(airport.LAT, airport.LONG);
                    var distance = CalculateDistance(startlocation, destinationlocation, airportLocation);



                    if (distance <= maxDistance)
                    {
                        airportsInRange.Add(airport);
                        airinfo a = new airinfo();
                        a.IATACODE = airport.IATACODE;
                        a.CITY = airport.CITY;
                        a.AIRPORTNAME = airport.AIRPORTNAME;
                        a.distance = distance;
                        airinrange.Add(a);
                    }
                }
                airinrange = airinrange.OrderBy(a => a.distance).ToList();
                return View("AirportDisplay", airinrange);
            }

        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }



        public ActionResult StateWiseAirports()
        {
            var airports = db.airportinfoes.ToList();
            List<String> list = airports.Select(m => m.STATE).Distinct().ToList();
            List<State> slist = new List<State>();
            foreach (var item in list)
            {
                slist.Add(new State { Sname = item });
            }



            var newList = slist.OrderBy(x => x.Sname);



            return View(newList);
        }



        [HttpPost]
        public ActionResult StateWiseAirports(string state)
        {
            var airports = db.airportinfoes.ToList();
            List<String> list = airports.Select(m => m.STATE).Distinct().ToList();
            List<State> slist = new List<State>();
            foreach (var item in list)
            {
                slist.Add(new State { Sname = item });
            }
            var StateList = slist.Where(x => x.Sname.Contains(state)).ToList();



            return View(StateList);

        }



        public ActionResult AirportList(string id)
        {
            var airports = db.airportinfoes.ToList();
            List<airportinfo> list = airports.FindAll(m => m.STATE == id);



            return View(list);



        }
        public double CalculateDistance(Location startLocation, Location destinationLocation, Location airportLocation)
        {
            var startToAirportDistance = HaversineDistance(startLocation, airportLocation);
            var airportToDestinationDistance = HaversineDistance(airportLocation, destinationLocation);
            var totalDistance = startToAirportDistance + airportToDestinationDistance;



            return totalDistance;
        }



        public double HaversineDistance(Location location1, Location location2)
        {



            //double dist;
            //var theta = location1.Longitude - location2.Longitude;



            //double distance = Math.Sin(DegreesToRadians(location1.Latitude)) * Math.Sin(DegreesToRadians(location2.Longitude)) * Math.Cos(DegreesToRadians(location1.Latitude))
            //    * Math.Cos(DegreesToRadians(location2.Latitude)) * Math.Cos(DegreesToRadians(theta));



            ////dist = Math.sin(deg2rad(lat1)) * Math.sin(deg2rad(lat2)) + Math.cos(deg2rad(lat1)) * Math.cos(deg2rad(lat2)) * Math.cos(deg2rad(theta));



            //distance = Math.Acos(distance);



            //dist = RadiansToDegree(distance);



            //dist = dist * 60 * 1.1515;



            //dist = dist * 1.609344;



            //return (dist);



            var earthRadius = 6371; // Radius of the Earth in kilometers
            var latitudeDifference = DegreesToRadians(location2.Latitude - location1.Latitude);
            var longitudeDifference = DegreesToRadians(location2.Longitude - location1.Longitude);



            var a = Math.Sin(latitudeDifference / 2) * Math.Sin(latitudeDifference / 2) +
            Math.Cos(DegreesToRadians(location1.Latitude)) * Math.Cos(DegreesToRadians(location2.Latitude)) *
            Math.Sin(longitudeDifference / 2) * Math.Sin(longitudeDifference / 2);



            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));



            var distance = earthRadius * c;



            return distance;
        }



        public double DegreesToRadians(double degrees)
        {



            return degrees * (Math.PI / 180);
        }
        public double RadiansToDegree(double degrees)
        {

            return degrees * (180 / Math.PI);
        }
        public ActionResult Services()
        {
            return View();
        }



    }



}