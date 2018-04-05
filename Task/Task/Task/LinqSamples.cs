// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;
using static ObjectDumper;

// Version Mad01

namespace SampleQueries
{
    [Title("LINQ Module")]
    [Prefix("Linq")]
    public class LinqSamples : SampleHarness
    {

        private DataSource dataSource = new DataSource();

        [Category("Restriction Operators")]
        [Title("Where - Task 1")]
        [Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
        public void Linq1()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var lowNums =
                from num in numbers
                where num < 5
                select num;

            Console.WriteLine("Numbers < 5:");
            foreach (var x in lowNums)
            {
                Console.WriteLine(x);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 2")]
        [Description("This sample return return all presented in market products")]

        public void Linq2()
        {
            var products =
                from p in dataSource.Products
                where p.UnitsInStock > 0
                select p;

            foreach (var p in products)
            {
                Write(p);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 5")]
        [Description("The list of Customers beginned from date when their become client sorted by Year, Month, money turnover, User Id")]
        public void Linq5()
        {
            var customers = dataSource.Customers.Where(c => c.Orders.Any())
                .Select(customer => new
                {
                    CustomerId = customer.CustomerID,
                    StartDate = customer.Orders.OrderBy(order => order.OrderDate)
                    .Select(ord => ord.OrderDate)
                    .First(),
                    MoneyTurnover = customer.Orders.Sum(order => order.Total)
                }).OrderByDescending(c => c.StartDate.Year)
                .ThenByDescending(c => c.StartDate.Month)
                .ThenByDescending(c => c.MoneyTurnover)
                .ThenByDescending(c => c.CustomerId);

            foreach (var c in customers)
            {
                Write($"Customer Id : {c.CustomerId} money turnover: {c.MoneyTurnover} Month : {c.StartDate.Month} Year : {c.StartDate.Year}");
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 7")]
        [Description("Product groups contained in stock sorted by price")]

        public void Linq7()
        {
            var productCategories = dataSource.Products
                .GroupBy(product => product.Category)
                .Select(group => new
                {
                    Category = group.Key,
                    StockedProducts = group.GroupBy(prod => prod.UnitsInStock > 0)
                        .Select(stockedProduct => new
                        {
                            ContainsInStock = stockedProduct.Key,
                            Products = stockedProduct.OrderBy(p => p.UnitPrice)
                        })
                });

            foreach (var category in productCategories)
            {
                Write($"Category: {category.Category}");
                foreach (var product in category.StockedProducts)
                {
                    if (product.ContainsInStock)
                    {
                        foreach (var containedProduct in product.Products)
                        {
                            Write($"Containing products: {containedProduct.ProductID}");
                        }
                    }
                    else
                    {
                        foreach (var notContainedProduct in product.Products)
                            Write($"Not containing products: {notContainedProduct.ProductID}");
                    }
                }
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 10")]
        [Description("Clients activity statistic by month, by year and by year and month")]
        public void Linq10()
        {
            var statistics = dataSource.Customers
                .Select(customer => new
                {
                    customer.CustomerID,
                    MonthsStatistic = customer.Orders.GroupBy(order => order.OrderDate.Month)
                        .Select(ord => new
                        {
                            Month = ord.Key,
                            OrdersCount = ord.Count()
                        }),

                    YearsStatistic = customer.Orders.GroupBy(order => order.OrderDate.Year)
                        .Select(g => new
                        {
                            Year = g.Key,
                            OrdersCount = g.Count()
                        }),

                    YearMonthStatistic = customer.Orders
                        .GroupBy(order => new
                        {
                            order.OrderDate.Year,
                            order.OrderDate.Month
                        })
                        .Select(ord => new
                        {
                            ord.Key.Year,
                            ord.Key.Month,
                            OrdersCount = ord.Count()
                        })
                });

            foreach (var statistic in statistics)
            {
                Write($"Customer Id: {statistic.CustomerID}");
                Write("Statistic by months:");
                foreach (var ms in statistic.MonthsStatistic)
                {
                    Write($"Month: {ms.Month} Activity: {ms.OrdersCount}");
                }
                Write("Statistic by years:");
                foreach (var ys in statistic.YearsStatistic)
                {
                    Write($"Year: {ys.Year} Activity: {ys.OrdersCount}");
                }
                Write("Statistic by ear and month:");
                foreach (var ym in statistic.YearMonthStatistic)
                {
                    Write($"Year: {ym.Year} Month: {ym.Month} Activity: {ym.OrdersCount}");
                }
            }
        }
    }
}
