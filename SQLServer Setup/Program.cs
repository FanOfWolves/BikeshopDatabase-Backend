﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLServer_Setup {
	class Program {
		static void Main(string[] args) {
			using(DBAccess database = new DBAccess()) {
				//database.DropAllTables();
				database.RunScript("Bike_Shop.sql");
				database.LoadData();

			}
			
			
			
		}

		static void PrintArray(String[] arr) {
			foreach(String i in arr) {
				Console.Write($"{i} ");
			}
		}
	}
}
