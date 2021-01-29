# 此库主要用于在使用EntityFramework查询时做一些表达式相关扩展

* 我的项目中，平常都会将EF的一些查询定义出来，如下所示：

```
public static Expression<Func<EquipmentInfo, SimpleEquipmentInfo>> SimpleEquipmentInfoExpression = s => new SimpleEquipmentInfo
        {
            Id = s.Id,
            Name = s.Name
        };
```

* 当我进行查询时：
```
_context.EquipmentInfo.Select(SimpleEquipmentInfoExpression).ToList();
```

* 我的当前做法是这样，可以复用一些表达式，不需要每次都写同样的表达式。
* 但是我又遇到了其他的问题，假设我想扩展一个属性,如下所示:
```
public class InfoExtCategoryName : SimpleEquipmentInfo
    {
        public string CategoryName { get; set; }
    }
```
* 此时我必须再写一个表达式，如下:
```
public static Expression<Func<EquipmentInfo, InfoExtCategoryName>> InfoExtCategoryName
            = s => new InfoExtCategoryName
            {
                Id = s.Id,
                Name = s.Name,
                CategoryName = s.EquipmentCategoryNav.Name
            };
```
* 这样就造成了重复的代码，于是我就打算扩展一个Merge方法来解决这个问题。
* 使用方法：
1. 引入命名空间
```
using EntityFrameworkCore.ExpressionHelp;
```
2. 调用扩展方法
```
public static Expression<Func<EquipmentInfo, InfoExtCategoryName>> InfoExtCategoryNameByMerge = 
SimpleEquipmentInfoExpression.Merge(s => new InfoExtCategoryName { CategoryName = s.EquipmentCategoryNav.Name});
```

