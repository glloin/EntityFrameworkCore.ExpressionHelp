using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityFrameworkCore.ExpressionHelp;

namespace SimpleDAL
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options):base(options)
        {

        }

        public DbSet<EquipmentInfo> EquipmentInfo { get; set; }
        public DbSet<EquipmentCategory> EquipmentCategory { get; set; }

    }



    public class EquipmentInfoService
    {
        private readonly Context _context;

        public EquipmentInfoService(Context context)
        {
            _context = context;
        }

        public static Expression<Func<EquipmentInfo, SimpleEquipmentInfo>> SimpleEquipmentInfoExpression = s => new SimpleEquipmentInfo
        {
            Id = s.Id,
            Name = s.Name
        };

        public List<SimpleEquipmentInfo> GetAll()
        {
            return _context.EquipmentInfo.Select(SimpleEquipmentInfoExpression).ToList();
        }

        public List<InfoExtCategoryName> GetInfoExtCategoryNames()
        {
            return _context.EquipmentInfo.Select(InfoExtCategoryNameExpression).ToList();
        }

        public List<InfoExtCategoryName> GetInfoExtCategoryNamesMerge()
        {
            return _context.EquipmentInfo.Select(InfoExtCategoryNameByMerge).ToList();
        }

        public static Expression<Func<EquipmentInfo, InfoExtCategoryName>> InfoExtCategoryNameByMerge = SimpleEquipmentInfoExpression.Merge(s => new InfoExtCategoryName { CategoryName = s.EquipmentCategoryNav.Name});

        public static Expression<Func<EquipmentInfo, InfoExtCategoryName>> InfoExtCategoryNameExpression
            = s => new InfoExtCategoryName
            {
                Id = s.Id,
                Name = s.Name,
                CategoryName = s.EquipmentCategoryNav.Name
            };




    }

    public class SimpleEquipmentInfo
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class InfoExtCategoryName : SimpleEquipmentInfo
    {
        public string CategoryName { get; set; }
    }

    public class EquipmentInfo
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public EquipmentCategory EquipmentCategoryNav { get; set; }
    }

    public class EquipmentCategory
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public ICollection<EquipmentInfo> EquipmentInfos { get; set; }

    }


}
