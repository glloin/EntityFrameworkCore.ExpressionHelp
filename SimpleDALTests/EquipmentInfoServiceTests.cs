using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleDAL;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace SimpleDAL.Tests
{
    [TestClass()]
    public class EquipmentInfoServiceTests
    {
        [TestMethod()]
        public void MergeExpressionTest()
        {
            // arrange
            var options = new DbContextOptionsBuilder<Context>().UseInMemoryDatabase("test data base").Options;
            var context = new Context(options);
            var service = new EquipmentInfoService(context);

            var eq1 = new EquipmentCategory { Id = 1, Name = "test name" };
            context.EquipmentCategory.Add(eq1);
            context.EquipmentInfo.Add(new EquipmentInfo { EquipmentCategoryNav = eq1, Name = "asds", Id = 1 });
            // act
            context.SaveChanges();
            var res1 = service.GetInfoExtCategoryNames().First();
            var res2 = service.GetInfoExtCategoryNamesMerge().First();
            // assert
            Assert.IsTrue(res1.CategoryName == eq1.Name);
            Assert.IsTrue(res2.CategoryName == eq1.Name);
            ;
        }
    }
}