# YS.Knife.Query 使用帮助文档

## 概述

YS.Knife.Query 是一个用于构建动态 LINQ 查询的 .NET 库。它提供了灵活的查询表达式解析和执行能力，支持过滤、排序、选择和聚合等操作。

## 核心功能

1. **Filter（过滤）**: 支持多种比较操作符
2. **OrderBy（排序）**: 支持字段升序/降序排序
3. **Select（选择）**: 支持选择特定字段
4. **Agg（聚合）**: 支持聚合函数

## 快速开始

### 安装

通过 NuGet 安装：

```bash
Install-Package YS.Knife.Query
```

### 基本使用

```csharp
using YS.Knife.Query;

// 创建查询信息
var queryInfo = new LimitQueryInfo
{
    Filter = "Age > 18 and Status = 'Active'",
    OrderBy = "CreateTime.desc(), Name.asc()",
    Select = "Id,Name,Age",
    Offset = 0,
    Limit = 10,
    Agg = "Amount.sum().as(total), Count.count().as(count)"
};

// 执行查询
var result = dbContext.Users.AsQueryable().QueryPage(queryInfo);
```

## 过滤表达式（Filter）

### 支持的操作符

| 操作符 | 别名 | 说明 | 示例 |
|--------|------|------|------|
| `==` | `=` | 等于 | `Age == 18` |
| `!=` | `<>` | 不等于 | `Status != 'Active'` |
| `>` | - | 大于 | `Score > 90` |
| `>=` | - | 大于等于 | `Price >= 100` |
| `<` | - | 小于 | `Age < 30` |
| `<=` | - | 小于等于 | `Price <= 999` |
| `bt` | `between` | 区间 | `Score bt [60, 100]` |
| `nbt` | `not_between` | 不在区间 | `Score nbt [0, 60]` |
| `in` | - | 在列表中 | `Status in ['Active', 'Pending']` |
| `nin` | `not_in` | 不在列表中 | `Role nin ['Admin']` |
| `sw` | `startswith` | 前缀匹配 | `Name sw '张'` |
| `nsw` | `not_startswith` | 不匹配前缀 | `Name nsw '李'` |
| `ew` | `endswith` | 后缀匹配 | `Email ew '@gmail.com'` |
| `new` | `not_endswith` | 不匹配后缀 | `Email new '@test.com'` |
| `ct` | `contains` | 包含 | `Name ct '伟'` |
| `nct` | `not_contains` | 不包含 | `Name nct 'test'` |

### 值类型

- **字符串**: 使用双引号或单引号，如 `"hello"` 或 `'world'`
- **数字**: 支持整数和浮点数，如 `1`, `1.5`, `-10`, `1_234_567`
- **布尔**: `true` 或 `false`
- **空值**: `null`
- **数组**: 使用方括号，如 `[1, 2, 3]`, `["a", "b"]`

### 组合表达式

使用 `and` 和 `or` 组合多个条件：

```plaintext
// AND 组合
Age > 18 and Status == 'Active'

// OR 组合
Role == 'Admin' or Role == 'Manager'

// 优先级控制（使用括号）
(Age > 18 and Age < 30) or Status == 'VIP'
```

### 属性路径

支持嵌套属性访问：

```plaintext
User.Name == '张三'
User.Address.City == '北京'
```

### 函数调用

支持在表达式中调用函数：

```plaintext
// 无参数
now() > CreateTime

// 带参数
addDay(CreateTime, 7) > now()
```

## 排序表达式（OrderBy）

### 基本语法

```plaintext
// 升序（默认）
FieldName.asc()

// 降序
FieldName.desc()

// 多字段排序
Name.asc(), CreateTime.desc()
```

### 示例

```plaintext
// 按创建时间降序
CreateTime.desc()

// 按姓名升序，年龄降序
Name.asc(), Age.desc()

// 使用函数排序
random().asc()
```

## 选择表达式（Select）

### 基本语法

```plaintext
// 选择多个字段
Id,Name,Age

// 选择嵌套对象
User.Name, User.Address.City
```

### 示例

```csharp
var selectInfo = SelectInfo.Parse("Id,Name,Age");
var result = queryable.DoSelect(selectInfo);
```

## 聚合表达式（Agg）

### 支持的聚合函数

| 函数 | 说明 | 示例 |
|------|------|------|
| `sum()` | 求和 | `Amount.sum()` |
| `avg()` | 平均值 | `Score.avg()` |
| `min()` | 最小值 | `Price.min()` |
| `max()` | 最大值 | `Price.max()` |
| `count()` | 计数 | `Id.count()` |
| `distinctCount()` | 去重计数 | `UserId.distinctCount()` |

### 别名

使用 `.as(alias)` 为聚合结果设置别名：

```plaintext
Amount.sum().as(totalAmount)
Score.avg().as(averageScore)
```

### 多个聚合

```plaintext
Amount.sum().as(total), Count.count().as(count), Score.avg().as(avgScore)
```

## API 参考

### QueryableExtensions

#### DoQuery
执行查询（过滤、排序、选择）

```csharp
IQueryable<T> DoQuery<T>(this IQueryable<T> source, QueryInfo queryInfo)
    where T : class, new()
```

#### QueryList
执行分页查询，返回列表

```csharp
List<T> QueryList<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
    where T : class, new()
```

#### QueryPage
执行分页查询，返回分页结果

```csharp
PagedList<T> QueryPage<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
    where T : class, new()
```

#### QueryLimitList
执行分页查询，返回限制列表（包含是否有更多数据）

```csharp
LimitList<T> QueryLimitList<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
    where T : class, new()
```

### FilterInfo

#### Parse
解析过滤表达式

```csharp
FilterInfo Parse(string filterExpression)
```

#### CreateItem
创建单个过滤条件

```csharp
FilterInfo CreateItem(string fieldPaths, Operator filterType, object value)
```

#### CreateAnd / CreateOr
创建组合过滤条件

```csharp
FilterInfo CreateAnd(params FilterInfo[] items)
FilterInfo CreateOr(params FilterInfo[] items)
```

### OrderByInfo

#### Parse
解析排序表达式

```csharp
OrderByInfo Parse(string orderText)
```

### SelectInfo

#### Parse
解析选择表达式

```csharp
SelectInfo Parse(string selectText)
```

### AggInfo

#### Parse
解析聚合表达式

```csharp
AggInfo Parse(string aggText)
```

## 数据类型转换

框架自动处理以下类型转换：

- **数字类型**: int, long, double, decimal 之间自动转换
- **字符串转日期**: `"2024-07-08"` 自动转换为 DateTime
- **字符串转 Guid**: `"c7bd06e4-dffb-4110-860c-9dc36523e9a9"` 自动转换为 Guid
- **枚举**: 支持字符串或数字转换

## 示例

### 示例 1：基本查询

```csharp
var queryInfo = new LimitQueryInfo
{
    Filter = "Age > 18 and Status == 'Active'",
    OrderBy = "CreateTime.desc()",
    Offset = 0,
    Limit = 10
};

var result = dbContext.Users.QueryPage(queryInfo);
```

### 示例 2：带聚合的查询

```csharp
var queryInfo = new LimitQueryInfo
{
    Filter = "Status == 'Completed'",
    Agg = "Amount.sum().as(total), Id.count().as(count)"
};

var result = dbContext.Orders.QueryPage(queryInfo);
// result.AggResult 包含聚合结果
```

### 示例 3：复杂过滤条件

```csharp
var filter = FilterInfo.Parse(
    "(Price >= 100 and Price <= 1000) or " +
    "Category in ['Electronics', 'Books']"
);
var result = dbContext.Products.DoFilter(filter).ToList();
```

### 示例 4：动态构建过滤条件

```csharp
var filter = FilterInfo.CreateItem("Name", Operator.Contains, "张");
filter = filter.AndAlso("Age", Operator.GreaterThan, 18);
filter = filter.OrElse(FilterInfo.CreateItem("Status", Operator.Equals, "VIP"));

var result = dbContext.Users.DoFilter(filter).ToList();
```

## 异常处理

### ParseException
当表达式语法错误时抛出

```csharp
try
{
    var filter = FilterInfo.Parse("invalid expression");
}
catch (ParseException ex)
{
    Console.WriteLine($"解析错误: {ex.Message}");
}
```

## 注意事项

1. **表达式语法**: 操作符前后需要有空格，如 `a > 1` 而非 `a>1`
2. **字符串转义**: 字符串中的双引号需要转义，如 `"He said \"Hello\""`
3. **性能考虑**: 复杂表达式可能影响查询性能，建议合理使用索引
4. **安全性**: 避免直接使用用户输入作为查询表达式，防止注入攻击

## 版本历史

- **1.0.0**: 初始版本，支持基本查询功能
- **1.1.0**: 添加聚合函数支持
- **1.2.0**: 增强类型转换能力

## 许可证

MIT License