# YS.Knife.Query

一个用于 **.NET 6** 的动态 LINQ 查询库：把字符串形式的查询条件（过滤、排序、选择、聚合、分页）解析并转换为 LINQ 表达式树，直接作用于 `IQueryable<T>`，可与 Entity Framework Core 等 LINQ Provider 配合使用。

![build](https://github.com/yscorecore/YS.Knife.Query/actions/workflows/build.yml/badge.svg)
[![Nuget](https://img.shields.io/nuget/v/YS.Knife.Query)](https://nuget.org/packages/YS.Knife.Query/)
[![GitHub](https://img.shields.io/github/license/yscorecore/YS.Knife.Query)](https://github.com/yscorecore/YS.Knife.Query/blob/master/LICENSE)

## 特性

- **Filter**：丰富的比较操作符、`and`/`or` 组合、嵌套属性路径、函数调用、自动类型转换
- **OrderBy**：多字段升序/降序、支持函数排序
- **Select**：字段裁剪、嵌套对象投影
- **Agg**：`sum` / `avg` / `min` / `max` / `count` / `distinctCount`，支持别名与聚合内过滤
- **分页**：`Offset` / `Limit`、`HasNext` 探测、可选总行数统计
- **同步 + 异步**：核心库提供同步 API，`YS.Knife.Query.EFCore` 提供基于 EF Core 的异步 API
- **强类型模型**：`QueryInfo` / `LimitQueryInfo` 使用 `record` 与 DataAnnotations，可直接用于 ASP.NET Core 参数绑定
- **表达式构建器**：由 C# Lambda 反向生成过滤字符串
- **可扩展函数**：自定义静态函数与实例函数，并支持 Provider 专属函数（如 `EF.Functions.Like`）

## 目录

- [项目结构](#项目结构)
- [安装](#安装)
- [快速开始](#快速开始)
- [核心模型](#核心模型)
- [查询 API](#查询-api)
- [过滤表达式（Filter）](#过滤表达式filter)
- [排序表达式（OrderBy）](#排序表达式orderby)
- [选择表达式（Select）](#选择表达式select)
- [聚合表达式（Agg）](#聚合表达式agg)
- [分页表达式（Limit）](#分页表达式limit)
- [函数](#函数)
- [类型转换](#类型转换)
- [表达式构建器（Builder）](#表达式构建器builder)
- [API 速查](#api-速查)
- [异常处理](#异常处理)
- [安全与性能建议](#安全与性能建议)
- [已知限制](#已知限制)
- [构建与测试](#构建与测试)
- [示例项目](#示例项目)

## 项目结构

```text
YS.Knife.Query/
├─ src/
│  ├─ YS.Knife.Query/               # 核心库（net6）
│  │  ├─ Filter/                    # FilterInfo、Operator、各操作符实现
│  │  ├─ OrderBy/                   # OrderByInfo、OrderByItem
│  │  ├─ Select/                    # SelectInfo、SelectItem
│  │  ├─ Agg/                       # AggInfo、AggItem、聚合执行
│  │  ├─ Limit/                     # LimitInfo、LimitList、PagedList
│  │  ├─ Parser/                    # 字符串表达式解析器
│  │  ├─ Functions/                 # 内置静态/实例函数、集合函数
│  │  ├─ ValueConverters/           # 常量值类型转换
│  │  ├─ ExpressionConverters/      # 表达式类型转换
│  │  ├─ Expressions/               # 值表达式执行
│  │  └─ Builder/                   # Lambda -> 过滤字符串
│  └─ YS.Knife.Query.EFCore/        # EF Core 异步扩展
├─ test/
│  ├─ YS.Knife.Query.UnitTest/      # 解析器、模型、转换器、Builder 单元测试
│  └─ YS.Knife.Query.IntegrationTest/ # 操作符、函数、排序、选择、聚合集成测试
├─ example/                          # ASP.NET Core + EF Core(SQLite) 示例
├─ docs/HELP.md
└─ README.md
```

## 安装

```bash
# 核心库
dotnet add package YS.Knife.Query

# EF Core 异步扩展（按需）
dotnet add package YS.Knife.Query.EFCore
```

或使用 Package Manager：

```powershell
Install-Package YS.Knife.Query
Install-Package YS.Knife.Query.EFCore
```

## 快速开始

### 1. 定义查询参数模型

`LimitQueryInfo` 已经内置了过滤、排序、选择、聚合与分页字段，可以直接作为 Web API 参数：

```csharp
[ApiController]
[Route("api/[controller]")]
public class MaterialController : ControllerBase
{
    private readonly IMaterialService materialService;

    public MaterialController(IMaterialService materialService)
    {
        this.materialService = materialService;
    }

    [HttpGet]
    public Task<PagedList<MaterialInfo>> QueryMaterials([FromQuery] LimitQueryInfo queryInfo)
    {
        return materialService.QueryMaterials(queryInfo);
    }
}
```

请求示例：

```text
GET /api/Material?Offset=0&Limit=20&OrderBy=CreatedAt.desc()
    &Filter=(Price%20%3E%3D%2010)%20and%20(Unit%20in%20%5B1%2C2%5D)
    &Agg=Price.sum().as(total),Id.count().as(count)
```

### 2. 执行查询

同步：

```csharp
using YS.Knife.Query;

var result = dbContext.Materials.QueryPage(queryInfo, q => q.Select(p => new MaterialInfo
{
    Id = p.Id,
    Name = p.Name,
    Price = p.Price
}));
```

异步（EF Core）：

```csharp
using YS.Knife.Query;

public Task<PagedList<MaterialInfo>> QueryMaterials(LimitQueryInfo queryInfo)
{
    return context.Materials.QueryPageAsync(queryInfo, q => q.Select(p => new MaterialInfo
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price
    }));
}
```

也可以分步执行：

```csharp
var query = dbContext.Materials
    .DoFilter(FilterInfo.Parse("(Price >= 10) and (Unit in [1,2])"))
    .DoOrderBy(OrderByInfo.Parse("CreatedAt.desc()"))
    .DoSelect(SelectInfo.Parse("Id,Name,Price"));

var list = query.Skip(20).Take(10).ToList();

// 或者直接使用分页扩展
var page = query.QueryLimitList(new LimitQueryInfo { Offset = 20, Limit = 10 });
```

## 核心模型

### QueryInfo

| 属性 | 类型 | 说明 |
| --- | --- | --- |
| `Filter` | `string` | 过滤表达式，最大长度 4096 |
| `OrderBy` | `string` | 排序表达式，最大长度 1024 |
| `Select` | `string` | 选择表达式，最大长度 1024 |
| `Distinct` | `bool` | 是否对最终结果去重，默认 `false` |

`QueryInfo` 是 `record`，支持 `with` 表达式。`QueryInfoExtensions` 提供：

```csharp
queryInfo = queryInfo.AppendFilter("Age > 18");   // 追加条件，使用 and 组合
queryInfo = queryInfo.PrependFilter("Status == \"Active\"");
```

`AsLimit(int limitCount = 10000)` 可把 `QueryInfo` 转换为 `LimitQueryInfo`，此时 `Offset = 0`，仅复制 `Filter` / `OrderBy` / `Select`（`Distinct` 不会被复制）。

### LimitQueryInfo

继承 `QueryInfo`，额外包含：

| 属性 | 类型 | 默认值 | 说明 |
| --- | --- | --- | --- |
| `Offset` | `int` | `0` | 跳过的行数，范围 `[0, int.MaxValue]` |
| `Limit` | `int` | `10000` | 最大返回行数，范围 `[0, 10000]` |
| `Agg` | `string` | `null` | 聚合表达式，最大长度 1024 |
| `CountAll` | `bool` | `true` | 是否统计总行数（`TotalCount`） |

### 返回模型

#### PagedList\<T>

| 属性 | 类型 | 说明 |
| --- | --- | --- |
| `Offset` | `int` | 当前偏移量 |
| `Limit` | `int` | 当前分页大小 |
| `HasNext` | `bool` | 是否还有下一页 |
| `TotalCount` | `long?` | 总行数；`CountAll = false` 时为 `null` |
| `Aggs` | `Dictionary<string, object>` | 聚合结果，无聚合时为 `null` |
| `Items` | `List<T>` | 当前页数据 |

`IPagedList` 还提供计算属性：

- `PageIndex`：`Limit > 0 ? Offset / Limit + 1 : 1`
- `PageSize`：等于 `Limit`
- `CanToPage`：`Limit > 0 && Offset % Limit == 0`

#### LimitList\<T>

| 属性 | 类型 | 说明 |
| --- | --- | --- |
| `Items` | `List<T>` | 数据 |
| `Offset` | `int` | 偏移量 |
| `Limit` | `int` | 限制数量 |
| `HasNext` | `bool` | 是否还有更多数据 |

可通过 `ToPagedList(totalCount, aggs)` 或 `ToPagedList(aggs)` 转为 `PagedList<T>`。

## 查询 API

### 同步 API（核心库）

| 方法 | 返回 | 说明 |
| --- | --- | --- |
| `DoQuery<T>(source, QueryInfo)` | `IQueryable<T>` | 只做过滤、排序、选择、去重，不执行查询 |
| `QueryList<T>(source, LimitQueryInfo)` | `List<T>` | `DoQuery` + `Skip` + `Take` |
| `QueryLimitList<T>(source, LimitQueryInfo)` | `LimitList<T>` | 多取一条判断 `HasNext`，不统计总数 |
| `QueryLimitList<T, R>(source, LimitQueryInfo, mapper)` | `LimitList<R>` | 投影后返回 |
| `QueryPage<T, R>(source, LimitQueryInfo, mapper)` | `PagedList<R>` | 分页 + 总数 + 聚合，`R` 必须为 `class` |
| `QueryPage<T>(source, LimitQueryInfo)` | `PagedList<T>` | 已标记 `[Obsolete]`，建议使用带 `mapper` 的重载 |

所有方法都要求 `T : class, new()`，且 `source` 不能为 `null`。

`DoQuery` 的执行顺序为：

```text
Filter -> OrderBy -> Select -> Distinct
```

### 异步 API（YS.Knife.Query.EFCore）

| 方法 | 返回 | 说明 |
| --- | --- | --- |
| `QueryListAsync<T>(source, queryInfo, cancellationToken)` | `Task<List<T>>` | 异步列表查询 |
| `QueryLimitListAsync<T, R>(source, queryInfo, mapper, cancellationToken)` | `Task<LimitList<R>>` | 异步限制列表查询 |
| `QueryPageAsync<T, R>(source, queryInfo, mapper, cancellationToken)` | `Task<PagedList<R>>` | 异步分页查询 |
| `QueryPageAsync<T>(source, queryInfo, cancellationToken)` | `Task<PagedList<T>>` | 已标记 `[Obsolete]` |

### 分页行为说明

1. **稳定排序**：`QueryLimitList` 与 `QueryPage` 会调用 `TryOrderByEntityKey()`。当查询没有显式排序时，会尝试按实体键补齐排序，键的查找顺序为：
   - 标记了 `[Key]` 的属性
   - 名称为 `Id` 的属性
   - 名称为 `{类型名}Id` 的属性

   找不到键时保持原样。该机制用于避免分页时数据重复或遗漏。
2. **总数优化**：`CountAll = true` 时，如果当前页返回行数大于 `0` 且小于 `Limit`，可直接用 `Offset + 行数` 作为总数，不再执行额外的 `COUNT` 查询。
3. **只统计总数**：`Limit <= 0` 时只执行 `LongCount()`，`Items` 为空列表。
4. **不统计总数**：`CountAll = false` 时 `TotalCount` 为 `null`，通过多取一条的方式计算 `HasNext`，聚合仍然执行。
5. **聚合范围**：聚合作用于「过滤后的全量数据」，不受 `Offset` / `Limit` / `Select` 影响。
6. **投影时机**：`CountAll = true` 时 `QueryPage` 的 `mapper` 在分页之后执行；`CountAll = false` 时先投影再分页。

### 服务接口

`IQueryService.cs` 提供两个服务抽象及扩展方法：

```csharp
public interface IQueryPageService<T>
{
    Task<PagedList<T>> QueryPage(LimitQueryInfo queryInfo);
}

public interface IQueryLimitListService<T>
{
    Task<LimitList<T>> QueryLimit(LimitQueryInfo queryInfo);
}
```

扩展方法：

```csharp
// IQueryPageService<T>
Task<long> CountAsync(QueryInfo queryInfo);
Task<List<T>> QueryListAsync(QueryInfo queryInfo, int limitCount = 10000);
Task<T> QueryOneAsync(QueryInfo queryInfo);

// IQueryLimitListService<T>
Task<List<T>> QueryListAsync(QueryInfo queryInfo, int limitCount = 10000);
Task<T> QueryOneAsync(QueryInfo queryInfo);
```

## 过滤表达式（Filter）

### 基本语法

单个条件由「左值 操作符 右值」组成，左值和右值都可以是常量、属性路径或函数：

```text
Price >= 100
Name sw "张"
Status in ["Active", "Pending"]
CreatedAt < Now()
```

空字符串、`null`、纯空白字符串解析结果为 `null`（不产生过滤条件）。

### 操作符

| 操作符 | 别名 | 枚举值 | 说明 |
| --- | --- | --- | --- |
| `==` | `=` | `Operator.Equals` | 等于 |
| `!=` | `<>` | `Operator.NotEquals` | 不等于 |
| `>` | - | `Operator.GreaterThan` | 大于 |
| `>=` | - | `Operator.GreaterThanOrEqual` | 大于等于 |
| `<` | - | `Operator.LessThan` | 小于 |
| `<=` | - | `Operator.LessThanOrEqual` | 小于等于 |
| `bt` | `between` | `Operator.Between` | 区间（闭区间，右值必须是 2 元素数组） |
| `nbt` | `not_between` | `Operator.NotBetween` | 不在区间 |
| `in` | - | `Operator.In` | 在数组中 |
| `nin` | `not_in` | `Operator.NotIn` | 不在数组中 |
| `sw` | `startswith` | `Operator.StartsWith` | 前缀匹配 |
| `nsw` | `not_startswith` | `Operator.NotStartsWith` | 不匹配前缀 |
| `ew` | `endswith` | `Operator.EndsWith` | 后缀匹配 |
| `new` | `not_endswith` | `Operator.NotEndsWith` | 不匹配后缀 |
| `ct` | `contains` | `Operator.Contains` | 包含 |
| `nct` | `not_contains` | `Operator.NotContains` | 不包含 |

操作符代码与关键字（`and` / `or` / `true` / `false` / `null`）**大小写不敏感**。

示例：

```text
Age >= 18
Price bt [100, 1000]
Status in ["Active", "Pending"]
Role nin ["Admin"]
Email ew "@gmail.com"
Name ct "伟"
```

### 值类型

| 类型 | 写法 | 说明 |
| --- | --- | --- |
| 字符串 | `"hello"` / `'world'` | 单双引号均可，支持 `\n`、`\t`、`\\`、`\"`、`\uXXXX` 等转义 |
| 整数 | `1`、`-10`、`+10`、`1_234_567` | 解析为 `int`，超出范围会抛异常 |
| 小数 | `1.5`、`-.1`、`+0.2`、`123_456.789` | 解析为 `double` |
| 布尔 | `true` / `false` | 大小写不敏感 |
| 空值 | `null` | 大小写不敏感；字符串 `"null"` 仍是字符串 |
| 数组 | `[1, 2, 3]`、`["a", "b"]`、`[true, null]` | 元素会统一为同一类型（必要时使用可空类型） |
| 空数组 | `[]` | 解析为空 `string[]` |

> 数字的小数点、正负号取自当前 `CultureInfo`；分组符号固定使用 `_`（避免与数组分隔符冲突）。需要指定区域时使用带 `CultureInfo` 参数的重载，例如 `FilterInfo.Parse(text, culture)`。

### 组合条件

**每个比较项必须用圆括号包裹**，再用 `and` / `or` 连接：

```text
(Age > 18) and (Status == "Active")
(Price >= 100) and (Price <= 1000)
(Role == "Admin") or (Role == "Manager")
```

嵌套组合需要再加一层括号：

```text
(Age > 18) and ((Status == "Active") or (Status == "Pending"))
(Age > 18) and ((Name sw "张") or (Name sw "李")) and (Score > 60)
```

优先级：`and` 高于 `or`，即：

```text
(a > 1) or (b < 2) and (c == 3) or (d != 4)
```

等价于：

```text
(a > 1) or ((b < 2) and (c == 3)) or (d != 4)
```

`FilterInfo.ToString()` 也会以这种标准格式输出，可通过 `TypeConverter` 完成字符串与 `FilterInfo` 的互转。

### 属性路径

使用 `.` 访问嵌套属性，点号两侧允许空白：

```text
User.Name == "张三"
User.Address.City == "北京"
a  .b   .c .d == 1
```

- 字段名首字符必须是字母或下划线，后续字符可以是字母、数字或下划线
- 属性名大小写不敏感
- 支持 `[JsonPropertyName]` 指定的别名
- 属性不存在时构建表达式会抛出异常

### 函数

函数分为两类：

1. **静态函数**：直接以函数名开头，例如 `Now()`、`Today()`、`NewGuid()`
2. **实例函数**：写在属性路径之后，例如 `CreatedAt.AddDays(7)`、`Name.ToLower()`、`Scores.Count()`

示例：

```text
CreatedAt < Now()
CreatedAt.AddDays(7) > Now()
Name.ToLower() == "abc"
Email.EndsWith("@gmail.com")
Scores.Count() > 3
Scores.Any(Score > 60)
Scores.Count(Score > 60) >= 2
```

说明：

- 函数名大小写不敏感
- 所有函数（包括无参函数）都必须带括号，例如 `Now()`、`CreatedAt.Year()`、`Name.Length()`、`Scores.Count()`；不加括号会被当作属性名
- `Any` / `Count` 的函数参数会被解析为过滤表达式，作用在集合元素上
- 内置函数见下文「函数」章节

### 动态构建过滤条件

```csharp
using YS.Knife.Query;

// 单条件
var filter = FilterInfo.CreateItem("Name", Operator.Contains, "张");

// 追加条件
filter = filter.AndAlso("Age", Operator.GreaterThan, 18);
filter = filter.OrElse(FilterInfo.CreateItem("Status", Operator.Equals, "VIP"));

// 组合
var complex = FilterInfo.CreateAnd(
    FilterInfo.CreateItem("Price", Operator.GreaterThanOrEqual, 100),
    FilterInfo.CreateItem("Price", Operator.LessThanOrEqual, 1000));

var result = dbContext.Users.DoFilter(filter).ToList();
```

`FilterInfo` 还提供：

- `Not()`：取反（单条件翻转操作符，`and` / `or` 组按德摩根定律转换）
- `AndAlso(FilterInfo)` / `AndAlso(field, op, value)`
- `OrElse(FilterInfo)` / `OrElse(field, op, value)`

### 判断单个对象是否匹配

```csharp
var entity = new User { Age = 20, Name = "张三" };

bool matched = entity.IsMatched("(Age > 18) and (Name sw \"张\")");
bool matched2 = entity.IsMatched(FilterInfo.Parse("Age > 18"));
```

## 排序表达式（OrderBy）

### 语法

多个排序项使用 `,` 或 `;` 分隔，每项格式为：

```text
属性路径[.asc()|.desc()]
```

不写方向时默认为升序。

```text
Name
Name.asc()
CreateTime.desc()
Name.asc(), Age.desc()
User.Address.City.desc(); Id.asc()
```

### 说明

- `asc` / `desc` 大小写不敏感
- 支持嵌套属性
- 支持函数作为排序项（如 `random()`，需已注册对应函数），但**不支持常量**
- 排序方向函数后不能再追加其他函数或属性
- 空字符串、`null`、纯空白解析结果为 `null`

### API

```csharp
var orderBy = OrderByInfo.Parse("Name, Age.desc()");
var ordered = queryable.DoOrderBy(orderBy);

// 代码构建
var orderBy2 = OrderByInfo.Create(
    new OrderByItem
    {
        NavigatePaths = new List<ValuePath> { new ValuePath { Name = "Name" } },
        OrderByType = OrderByType.Asc
    },
    new OrderByItem
    {
        NavigatePaths = new List<ValuePath> { new ValuePath { Name = "Age" } },
        OrderByType = OrderByType.Desc
    });

orderBy2.Add(new OrderByItem { /* ... */ });
```

## 选择表达式（Select）

### 语法

多个选择项使用 `,` 或 `;` 分隔：

```text
Id,Name,Age
User.Name,User.Address.City
```

子对象投影：

```text
Id,Name,Specs(FullName,Factor)
```

集合相关修饰符（`{...}`）：

```text
Specs{where(FullName ct "2")}
Specs{orderby(Factor.desc())}
Specs{limit(0,5)}
Specs{limit(0,5),orderby(Factor.desc()),where(Factor > 1)}(FullName)
```

### 语法元素

| 语法 | 说明 |
| --- | --- |
| `-abc` | `Exclude = true` |
| `+abc` | 显式包含（默认行为） |
| `[all]` | 特殊名称 `SpecialName.All` |
| `[default]` | 特殊名称 `SpecialName.Default` |
| `[complex]` | 特殊名称 `SpecialName.Complex` |
| `a(b,c)` | 嵌套对象选择，`SubItems` |
| `a{where(...)}` | 集合过滤，`CollectionFilter` |
| `a{orderby(...)}` | 集合排序，`CollectionOrderBy` |
| `a{limit(offset,limit)}` | 集合分页，`CollectionLimit` |

### 运行时行为

`DoSelect` 使用 `MemberInit` 构造新对象：

- 选择 `Id,Name` 时，返回的新对象只赋值 `Id`、`Name`，其余属性保持默认值
- 支持嵌套对象投影
- 空 `SelectInfo` / `SelectInfo.Items` 为空时不做投影，返回原始查询
- 目标属性必须可写，否则抛出异常
- 嵌套对象类型也需要无参构造函数
- 要求 `T : class, new()`，即实体必须有无参构造函数

> 注意：这些扩展语法目前只完成**语法解析**。`DoSelect` 的实际行为是：
>
> - `-abc` 虽然解析为 `Exclude = true`，但仍会投影 `abc`（排除语义未实现）
> - `[all]` / `[default]` / `[complex]` 会被当作普通属性名，找不到时抛出异常
> - 集合 `{where/orderby/limit}` 会被忽略，只应用 `(SubItems)` 子对象投影

### API

```csharp
var selectInfo = SelectInfo.Parse("Id,Name,Specs(FullName,Factor)");
var result = queryable.DoSelect(selectInfo);
```

## 聚合表达式（Agg）

### 语法

多个聚合项使用 `,` 或 `;` 分隔：

```text
Amount.sum()
Amount.sum().as(total)
Amount.sum().as("totalAmount")
Amount.sum(Status == "Paid").as(paidTotal)
Price.avg().as(avgPrice), Id.count().as(count)
```

- 不带聚合函数时默认使用 `sum`，例如 `Amount` 等价于 `Amount.sum()`
- 聚合函数名大小写不敏感
- 支持 `.as(alias)` 设置别名，别名可以是标识符、数字或带引号的字符串

### 聚合类型

| 类型 | 说明 |
| --- | --- |
| `Sum` | 求和 |
| `Min` | 最小值 |
| `Max` | 最大值 |
| `Avg` | 平均值 |
| `Count` | 行数统计（对过滤后的行计数） |
| `DistinctCount` | 对指定属性去重后计数 |

`Sum` / `Avg` 适用于数值类型，`Min` / `Max` 需要类型支持比较。

### 聚合内过滤

```text
Amount.sum(Status == "Paid").as(paidTotal)
```

组合条件同样需要使用括号：

```text
Amount.sum((Status == "Paid") and (Amount > 0)).as(paidTotal)
```

### 结果

```csharp
var aggs = queryable.DoAgg(AggInfo.Parse("Amount.sum().as(total), Id.count().as(count)"));
// aggs["total"] -> Sum 结果
// aggs["count"] -> Count 结果
```

在 `PagedList<T>` 中通过 `Aggs` 属性获取：

```csharp
var page = dbContext.Orders.QueryPage(queryInfo, q => q.Select(p => new OrderInfo()));
var total = page.Aggs?["total"];
```

### 说明

- 未设置别名时，结果键为 `agg0`、`agg1`……（从 0 开始）
- 单个查询最多支持 **64 个**聚合项，超出会抛出 `BuildException`
- 数据集为空时：`Sum` / `Count` / `DistinctCount` 返回 `0`，`Min` / `Max` / `Avg` 返回 `null`
- 聚合基于 `GroupBy(p => 1)` + `Select` 构建，字段计算使用 `Enumerable` 方法；不同 LINQ Provider 的翻译能力不同，聚合内过滤尤其可能无法翻译为 SQL，请以实际生成的 SQL 为准
- 空字符串、`null`、纯空白解析结果为 `null`

## 分页表达式（Limit）

`LimitInfo` 用于描述集合选择中的分页片段：

```text
limit(limit)
limit(offset,limit)
```

示例：

```text
limit(10)     => Offset = 0, Limit = 10
limit(20,10)  => Offset = 20, Limit = 10
```

```csharp
var limit = LimitInfo.Parse("20,10");
var query = source.Skip(limit.Offset).Take(limit.Limit);
```

`LimitInfo` 的构造函数会校验 `offset`、`limit` 不能为负数。

## 函数

### 内置静态函数

| 名称 | 说明 | 返回类型 |
| --- | --- | --- |
| `Now()` | `DateTime.Now` | `DateTime` |
| `UtcNow()` | `DateTime.UtcNow` | `DateTime` |
| `Today()` | `DateTime.Today` | `DateTime` |
| `NewGuid()` | `Guid.NewGuid()` | `Guid` |
| `Join(args)` | `string.Join(",", args)` | `string` |
| `IsNullOrEmpty(s)` | `string.IsNullOrEmpty(s)` | `bool` |
| `LatestWeek()` | 最近的星期日（从今天向前回溯） | `DateTime` |
| `LatestWeek1()` ~ `LatestWeek6()` | 最近的周一 ~ 周六（从今天向前回溯） | `DateTime` |
| `ThisMonth()` | 本月 1 日 | `DateTime` |
| `ThisYear()` | 本年 1 月 1 日 | `DateTime` |

> `LatestWeek*`、`ThisMonth`、`ThisYear` 是**在 `StaticFunctions` 静态构造时计算一次的常量**（`AddConstant`），长时间运行的服务中日期不会自动滚动。需要动态日期时请在请求处理时重新注册或改用 `Now()` 配合 `AddDays`。

### 内置实例函数

**DateTime**

| 名称 | 说明 |
| --- | --- |
| `AddYears(int)` | 加年 |
| `AddMonths(int)` | 加月 |
| `AddDays(double)` | 加天 |
| `AddHours(double)` | 加小时 |
| `AddMinutes(double)` | 加分钟 |
| `AddSeconds(double)` | 加秒 |
| `AddMilliseconds(double)` | 加毫秒 |
| `Year()` / `Month()` / `Day()` | 年 / 月 / 日 |
| `Hour()` / `Minute()` / `Second()` / `Millisecond()` | 时 / 分 / 秒 / 毫秒 |
| `DayOfWeek()` | 星期 |
| `Date()` | 日期部分 |

**string**

| 名称 | 说明 |
| --- | --- |
| `ToLower()` | 转小写 |
| `ToUpper()` | 转大写 |
| `CompareTo(string)` | 字符串比较 |
| `StartsWith(string)` | 是否以指定字符串开头 |
| `EndsWith(string)` | 是否以指定字符串结尾 |
| `Contains(string)` | 是否包含指定字符串 |
| `Length()` | 字符串长度 |

**集合（ICollection 及其实现，如 List、数组）**

| 名称 | 说明 |
| --- | --- |
| `Any()` / `Any(filter)` | 是否存在元素 / 是否存在满足条件的元素 |
| `Count()` / `Count(filter)` | 元素个数 / 满足条件的元素个数 |

### 自定义函数

**静态函数**：使用 `It.Arg<T>()` 占位参数。

```csharp
using YS.Knife.Query.Functions;

// 无参：random()
StaticFunctions.Add("random", () => Random.Shared.Next());

// 有参：randomInt(10)
StaticFunctions.Add("randomInt", () => Random.Shared.Next(0, It.Arg<int>()));
```

**实例函数**：

```csharp
// 注册 string.like(@s)：Name.like("%张%")
InstanceFunctions.Add<string, bool>("like", s => EF.Functions.Like(s, It.Arg<string>()));
```

若希望值在注册时固定（而不是每次查询重新求值），可使用 `AddConstant`：

```csharp
StaticFunctions.AddConstant("startOfToday", () => DateTime.Today);
```

注销与查询：

```csharp
StaticFunctions.Remove("random");
InstanceFunctions.Remove(typeof(string), "like");
bool exists = InstanceFunctions.Contains(typeof(string), "like");
```

### EF Core 集成示例

`example/YS.Knife.Query.Demo.AspnetCore/Program.cs` 中演示了注册 Provider 专属函数：

```csharp
StaticFunctions.Add("random", () => EF.Functions.Random());
InstanceFunctions.Add<string, bool>("like", s => EF.Functions.Like(s, It.Arg<string>()));
```

之后即可在查询字符串中使用：

```text
OrderBy=random()
Filter=Name.like("%食材%")
```

## 类型转换

过滤表达式在构建时会把左右两侧转换为同一类型，规则如下：

| 场景 | 行为 |
| --- | --- |
| 数字之间 | `int` / `long` / `short` / `double` / `decimal` / `float` 等通过 `Convert.ChangeType` 转换 |
| 字符串 → 数字 | 使用目标类型的 `Parse(string)` |
| 字符串 → `DateTime` / `DateTimeOffset` / `Guid` | 使用目标类型的 `Parse(string)` |
| 字符串 → 枚举 | 通过 `TypeDescriptor` 转换，如 `Status == "Active"` |
| 数字 → 枚举 | `Enum.ToObject`，如 `Status == 1` |
| 常量 → 成员 | 常量会被转换为成员的类型，如 `Id == "c7bd06e4-..."`（字符串转 `Guid`） |
| 非常量表达式 → `string` | 双方类型不同且都不是常量时，支持将表达式转为 `string`（`ToString()`） |
| `null` | 与引用类型直接比较；与非空值类型比较时会自动提升为对应的可空类型，如 `Age == null` 会按 `(int?)Age == null` 处理 |
| `In` 数组 | 数组元素会转换为左侧成员的类型 |

比较运算的特殊规则：

- 字符串的 `>` / `>=` / `<` / `<=` 使用 `string.Compare(left, right)` 与 `0` 比较
- 数值、`DateTime`、`DateTimeOffset`、`char`、`nint` 及自定义 `op_LessThan` 类型直接使用比较运算符
- `sw` / `ew` / `ct` 会先把两侧转换为 `string`；任一侧为 `null` 时正向操作符返回 `false`
- `bt` / `nbt` 的右值必须是常量数组且元素个数为 2

## 表达式构建器（Builder）

把已有的 Lambda 表达式反向生成为过滤字符串，便于复用强类型代码：

```csharp
using YS.Knife.Query;

string filter1 = Builder.CreateFilter<User>(p => p.Age > 18);
// "Age > 18"

string filter2 = Builder.CreateFilter<User>(p => p.Name == "张三");
// "Name == \"张三\""

string filter3 = Builder.CreateFilter<User>(p => p.Age > 18 && p.Age < 60);
// "(Age > 18) and (Age < 60)"

string filter4 = Builder.CreateFilter<User>(
    p => p.Name.StartsWith("张") && p.Age > 18);
// "(Name.StartsWith(\"张\") == true) and (Age > 18)"
```

生成的字符串可以直接交给 `FilterInfo.Parse` 再次解析（字符串、数字、枚举等常量可往返，详见下方限制）。

### 支持的常见写法

| C# 写法 | 生成的过滤文本 | 说明 |
| --- | --- | --- |
| `p.Age > 18` | `Age > 18` | `==` / `!=` / `>` / `>=` / `<` / `<=` |
| `p.Name == "张三"` | `Name == "张三"` | 字符串自动加引号 |
| `p.Name == abc`（局部变量） | `Name == "..."` | 捕获变量按常量处理 |
| `p.Name == null` / `p.NullableAge == null` | `Name == null` | null 常量 |
| `p.Address.City == "北京"` | `Address.City == "北京"` | 嵌套属性 |
| `p.Name.Length > 3` | `Name.Length > 3` | 属性链，运行时按属性解析 |
| `p.Tags.Count > 0` | `Tags.Count > 0` | 集合属性 |
| `p.CreatedAt.Year == 2024` | `CreatedAt.Year == 2024` | DateTime 属性 |
| `p.Age > 18 && p.Age < 60` | `(Age > 18) and (Age < 60)` | `&&` / `||` 可任意嵌套 |
| `p.IsActive == true` / `== false` | `IsActive == true` | 布尔比较 |
| `!p.IsActive` | `IsActive != true` | 取反布尔成员或布尔方法 |
| `p.Status == Status.Active` | `Status == 1` | 枚举输出数字，捕获的枚举变量同样 |
| `p.CreatedAt.DayOfWeek == DayOfWeek.Monday` | `CreatedAt.DayOfWeek == 1` | 枚举属性 |
| `p.Name.StartsWith("张")` | `Name.StartsWith("张") == true` | 布尔方法自动补 `== true` |
| `p.Name.ToLower() == "abc"` | `Name.ToLower() == "abc"` | 已注册的字符串实例函数 |
| `p.Name.CompareTo("abc") >= 0` | `Name.CompareTo("abc") >= 0` | 已注册的字符串实例函数 |
| `p.CreatedAt.AddDays(7) > p.UpdatedAt` | `CreatedAt.AddDays(7) > UpdatedAt` | 已注册的 DateTime 实例函数 |
| `arr.Contains(p.Name)`（`arr` 为数组/`List`/`HashSet`） | `Name in ["a","b"]` | `Enumerable.Contains` 与集合实例方法均支持 |
| `new[] { "a", "b" }.Contains(p.Name)` | `Name in ["a","b"]` | 数组字面量 |
| `!arr.Contains(p.Name)` | `Name nin ["a","b"]` | 取反转换为 `nin` |
| `p.Tags.Contains("a")` | `"a" in Tags` | 集合属性作为右值 |
| `p.Tags.Contains(p.Name)` | `Name in Tags` | 两侧都是属性路径 |
| `p.Scores.Any()` | `Scores.Any() == true` | 集合非空，布尔方法自动补 `== true` |
| `!p.Scores.Any()` | `Scores.Any() != true` | 取反等价于集合为空 |
| `p.Scores.Count() > 0` | `Scores.Count() > 0` | 集合数量比较 |
| `p.Scores.Count() == 3` | `Scores.Count() == 3` | 精确计数 |
| `p.Scores.Any(s => s.Age > 18)` | `Scores.Any(Age > 18) == true` | Lambda 反向为内部 Filter，参数名省略 |
| `p.Scores.Any(s => s.Age > 18 && s.Name != null)` | `Scores.Any((Age > 18) and (Name != null)) == true` | Lambda 内可嵌套 `&&` / `||` |
| `p.Scores.Count(s => s.Age > 18) > 0` | `Scores.Count(Age > 18) > 0` | 带 Lambda 的 Count 与比较运算符组合 |
| `p.Scores.Any(s => s.Address.City == "北京")` | `Scores.Any(Address.City == "北京") == true` | Lambda 内支持嵌套属性路径 |

常量类型支持：`string`、`bool`、`null`、数字（`int` / `long` / `decimal` / `double` 等）、枚举、`char`（输出数字）。`DateTime`、`Guid`、`TimeSpan` 等常量虽然能生成，但输出未加引号，**不能被 `FilterInfo.Parse` 回读**，不建议在 Builder 中使用。

### 不支持的常见写法

以下写法会抛出 `NotSupportedException`：

- **其他 LINQ / 静态方法**：`string.IsNullOrEmpty(p.Name)`、`arr.All(...)`、`Enumerable.Select(...)`
- **未注册的实例方法**：`p.Name.Equals("x")`、`p.Name.Trim()`、`p.Name.Substring(...)`、`p.Age.ToString()`
- **多参数 Lambda 或高级语法**：`p.Scores.Any((x, i) => ...)`、`p.Scores.SelectMany(...)`（Builder 仅支持 Any/Count 的单参数 Lambda）
- **索引器**：`p.Scores[0] == 1`
- **非短路逻辑**：`&`、`|`（只支持 `&&`、`||`）
- **二元运算**：`p.Name == "a" + captured`、`p.Age > 18 + offset`（编译器可折叠的纯常量运算除外，如 `18 + 2`）
- **其他节点**：`!(p.Age > 18)`、裸布尔属性 `p => p.IsActive`、三元表达式、`new DateTime(...)`、`DateTime.Parse(...)`

运行时过滤语法 `Scores.Any(Score > 60)` 中的 `Score` 直接是元素属性名，Builder 从 C# Lambda 反向生成时会自动把 Lambda 参数名去掉，输出等价的过滤字符串。

## API 速查

### FilterInfo

```csharp
FilterInfo.Parse(string expression);
FilterInfo.Parse(string expression, CultureInfo culture);
FilterInfo.CreateItem(string fieldPaths, Operator filterType, object value);
FilterInfo.CreateAnd(params FilterInfo[] items);
FilterInfo.CreateOr(params FilterInfo[] items);
FilterInfo.AndAlso(FilterInfo other);
FilterInfo.AndAlso(string fieldPaths, Operator filterType, object value);
FilterInfo.OrElse(FilterInfo other);
FilterInfo.OrElse(string fieldPaths, Operator filterType, object value);
FilterInfo.Not();
```

### ValueInfo

```csharp
ValueInfo.Parse(string valueExpression);
ValueInfo.Parse(string valueExpression, CultureInfo culture);
ValueInfo.FromConstantValue(object value);
ValueInfo.FromPaths(List<ValuePath> navigatePaths);
```

### OrderByInfo / SelectInfo / AggInfo / LimitInfo

```csharp
OrderByInfo.Parse(string orderText);
OrderByInfo.Create(params OrderByItem[] orderItems);
orderByInfo.Add(OrderByItem item);
orderByInfo.HasItems();

SelectInfo.Parse(string text);

AggInfo.Parse(string aggText);
aggInfo.Add(AggItem item);
aggInfo.HasItems();

LimitInfo.Parse(string limitStr);
new LimitInfo(int offset, int limit);
```

### 查询扩展

```csharp
IQueryable<T> DoFilter<T>(this IQueryable<T> source, FilterInfo filter);
IQueryable<T> DoOrderBy<T>(this IQueryable<T> source, OrderByInfo orderInfo);
IQueryable<T> DoSelect<T>(this IQueryable<T> source, SelectInfo selectInfo);
Dictionary<string, object> DoAgg<T>(this IQueryable<T> source, AggInfo aggInfo);
```

## 异常处理

| 异常 | 触发场景 |
| --- | --- |
| `ParseException` | 表达式语法错误；消息中会包含出错位置的索引 |
| `BuildException` | 表达式构建时找不到成员、操作符返回类型不是 `bool`、聚合项超过 64 个等 |
| `Exception` | 找不到属性/函数、类型无法转换、`Select` 目标属性不可写等；部分场景直接抛出基类 `Exception` |
| `ArgumentNullException` | `source`、`mapper` 等必需参数为 `null` |
| `ArgumentOutOfRangeException` | `LimitInfo` 的 `offset` / `limit` 为负数 |
| `NotSupportedException` | `Builder` 遇到不支持的表达式节点 |

```csharp
try
{
    var filter = FilterInfo.Parse("(Age > ) and");
}
catch (ParseException ex)
{
    Console.WriteLine($"解析失败: {ex.Message}");
}
```

## 安全与性能建议

1. **避免拼接 SQL**：本库把字符串解析为表达式树，不直接拼接 SQL，但字段名会映射到实体属性。仍建议对用户可输入的字段名做白名单校验，避免访问不应暴露的属性。
2. **限制查询规模**：`LimitQueryInfo.Limit` 的上限为 10000，`Offset` 无上限但过大时数据库性能会下降。
3. **合理使用索引**：过滤与排序字段应建立合适索引，尤其是分页查询的排序字段。
4. **谨慎使用总数统计**：大数据量下 `CountAll = true` 可能带来额外开销，可根据场景设为 `false`。
5. **函数翻译**：`Now()`、字符串函数、自定义函数等需要 LINQ Provider 支持翻译；EF Core 中建议使用 `EF.Functions.*` 注册 Provider 原生函数，并检查实际生成的 SQL。
6. **文化差异**：默认使用 `CurrentCulture` 解析数字；服务端建议显式传入固定的 `CultureInfo`，避免不同部署环境解析结果不一致。
7. **字符串必须加引号**：未加引号的标识符会被当作属性路径，例如 `Status == Active` 表示与名为 `Active` 的属性比较，而不是枚举字符串。

## 已知限制

- 组合条件要求每个比较项带圆括号，不支持省略括号的 `a > 1 and b < 2` 写法。
- `Select` 的 `-` 排除、`[all]` / `[default]` / `[complex]` 特殊名称、集合 `{where/orderby/limit}` 仅支持解析，`DoSelect` 尚未实现运行时应用。
- 聚合项最多 64 个。
- `Count` 聚合统计的是行数（属性路径仅用于语法占位），需要按属性去重计数请使用 `DistinctCount`。
- 部分操作依赖 Provider 翻译能力，内存查询与数据库查询的行为可能存在差异。
- `LatestWeek*` / `ThisMonth` / `ThisYear` 为静态构造时计算的常量。

## 构建与测试

```bash
# 还原依赖
dotnet restore

# 构建
dotnet build

# 运行全部测试
dotnet test
```

测试技术栈：xUnit + FluentAssertions + Bogus。

测试覆盖范围：

- `YS.Knife.Query.UnitTest`：过滤器/排序/选择/聚合/分页解析、`FilterInfo` 组合与取反、值转换、表达式转换、Builder
- `YS.Knife.Query.IntegrationTest`：16 种操作符在常量/属性路径/函数之间的组合、`Now` 等函数、排序、选择、聚合（含可空数值类型）

## 示例项目

```bash
dotnet run --project example/YS.Knife.Query.Demo.AspnetCore
```

示例使用 ASP.NET Core + EF Core + SQLite：

- 启动时创建 `demo.db` 并写入 200 条食材数据
- `GET /api/Material` 直接接收 `LimitQueryInfo`，演示过滤、排序、聚合与分页
- 演示如何注册 Provider 专属函数（`EF.Functions.Random`、`EF.Functions.Like`）

```text
GET /api/Material?Offset=0&Limit=10&OrderBy=CreatedAt.desc()&Filter=(Price%20%3E%3D%200.2)%20and%20(Unit%20in%20%5B0%2C1%5D)
```

## 相关文档

- [docs/HELP.md](docs/HELP.md)：早期使用说明（部分示例可能滞后，请以本 README 和源码为准）

## 许可证

[MIT](LICENSE)
