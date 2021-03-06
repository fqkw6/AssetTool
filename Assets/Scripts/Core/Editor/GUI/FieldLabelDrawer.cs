﻿/*===============================
 * Author: [Allen]
 * Purpose: 变量标签
 * Time: 2019/10/29 17:39:08
================================*/
using UnityEditor;
using UnityEngine;

//绑定特性描述类
[CustomPropertyDrawer(typeof(FieldLabelAttribute))]
public class FieldLabelDrawer : PropertyDrawer
{
    private FieldLabelAttribute FLAttribute
    {
        get { return (FieldLabelAttribute)attribute; }
        ////获取你想要绘制的字段
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //在这里重新绘制
        EditorGUI.PropertyField(position, property, new GUIContent(FLAttribute.label), true);
    }
}
