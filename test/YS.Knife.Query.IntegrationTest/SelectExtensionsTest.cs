using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;

namespace YS.Knife.Query.IntegrationTest
{
    public class SelectExtensionsTest
    {
        #region SampleEntity
        [Fact]
        public void ShouldReturnAllColumnsWhenSelectInfoIsNull()
        {
            var source = GetSampleEntityTestData();
            var result = source.DoSelect(null).ToList().First();
            result.Should().Be(new SampleEntity
            {
                Val1 = 1,
                Val2 = 2,
                Val3 = 3
            });
        }
        [Fact]
        public void ShouldReturnAllColumnsWhenSelectInfoIsEmpty()
        {
            var source = GetSampleEntityTestData();
            var selectInfo = new SelectInfo
            {
            };
            var result = source.DoSelect(selectInfo).ToList().First();
            result.Should().Be(new SampleEntity
            {
                Val1 = 1,
                Val2 = 2,
                Val3 = 3
            });
        }
        [Fact]
        public void ShouldReturnAllColumnsWhenSelectInfoItemsIsEmpty()
        {
            var source = GetSampleEntityTestData();
            var selectInfo = new SelectInfo
            {
                Items = new List<SelectItem>()
            };
            var result = source.DoSelect(selectInfo).ToList().First();
            result.Should().Be(new SampleEntity
            {
                Val1 = 1,
                Val2 = 2,
                Val3 = 3
            });
        }
        [Fact]
        public void ShouldReturnQueryedColumnsWhenQueryOneProperty()
        {
            var source = GetSampleEntityTestData();
            var selectInfo = new SelectInfo
            {
                Items = new List<SelectItem>
                {
                    new SelectItem{ Name=nameof(SampleEntity.Val1)},
                }
            };
            var result = source.DoSelect(selectInfo).ToList().First();
            result.Should().Be(new SampleEntity
            {
                Val1 = 1,
                Val2 = 0,
                Val3 = 0
            });
        }
        [Fact]
        public void ShouldReturnQueryedColumnsWhenQueryMultiProperties()
        {
            var source = GetSampleEntityTestData();
            var selectInfo = new SelectInfo
            {
                Items = new List<SelectItem>
                {
                    new SelectItem{ Name=nameof(SampleEntity.Val1)},
                    new SelectItem{ Name=nameof(SampleEntity.Val3)},
                }
            };
            var result = source.DoSelect(selectInfo).ToList().First();
            result.Should().Be(new SampleEntity
            {
                Val1 = 1,
                Val2 = 0,
                Val3 = 3
            });
        }
        private IQueryable<SampleEntity> GetSampleEntityTestData()
        {
            var entity = new SampleEntity
            {
                Val1 = 1,
                Val2 = 2,
                Val3 = 3
            };
            return new SampleEntity[] { entity }.AsQueryable();
        }
        #endregion

        [Fact]
        public void ShouldReturnNestedObject()
        {
            var source = GetNestedObjectEntityTestData();
            var selectInfo = new SelectInfo
            {
                Items = new List<SelectItem>
                {
                    new SelectItem{ Name=nameof(NestedObjectEntity.Sample)},
                }
            };
            var result = source.DoSelect(selectInfo).ToList().First();
            result.Should().Be(new NestedObjectEntity
            {
                Sample = new SampleEntity
                {
                    Val1 = 1,
                    Val2 = 2,
                    Val3 = 3
                }
            });
        }

        [Fact]
        public void ShouldReturnNestedObjectSubPropertyWhenQuerySubProperty()
        {
            var source = GetNestedObjectEntityTestData();
            var selectInfo = new SelectInfo
            {
                Items = new List<SelectItem>
                {
                    new SelectItem
                    {
                        Name=nameof(NestedObjectEntity.Sample),
                        SubItems = new List<SelectItem>
                        {
                            new SelectItem{ Name=nameof(SampleEntity.Val1),}
                        }
                    },
                }
            };
            var result = source.DoSelect(selectInfo).ToList().First();
            result.Should().Be(new NestedObjectEntity
            {
                Sample = new SampleEntity
                {
                    Val1 = 1,
                    Val2 = 0,
                    Val3 = 0
                }
            });
        }

        private IQueryable<NestedObjectEntity> GetNestedObjectEntityTestData()
        {
            var entity = new SampleEntity
            {
                Val1 = 1,
                Val2 = 2,
                Val3 = 3
            };
            var nested = new NestedObjectEntity
            {
                Prop1 = "prop1",
                Sample = entity
            };
            return new NestedObjectEntity[] { nested }.AsQueryable();
        }

        record SampleEntity
        {
            public int Val1 { get; set; }
            public int Val2 { get; set; }
            public int Val3 { get; set; }
        }
        record NestedObjectEntity
        {
            public string Prop1 { get; set; }
            public SampleEntity Sample { get; set; }
        }
    }
}
