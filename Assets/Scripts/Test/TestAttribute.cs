using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  作用域 类枚举等， 是否可继承，是否可修饰
/// 1、AttributeTargets
///表示这个属性类用在什么类型上面，枚举值
///2、AllowMultiple
/// 表示可否在一个类上声明多次，布尔值
/// 3、Inherited
/// 该属性是否可以被派生类继承，布尔值
/// </summary>
[System.AttributeUsage (System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]

public sealed class TestNameAttribute : Attribute {
    private readonly string _name;

    public string Name {
        get { return _name; }
    }

    public TestNameAttribute (string name) {
        _name = name;
    }

}

[TestName ("dept")]
public class CustomAttributes {
    [TestName ("Deptment Name")]
    public string Name { get; set; }

    [TestName ("Deptment Address")]
    public string Address;
}
