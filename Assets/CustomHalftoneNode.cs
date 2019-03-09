using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;
using System;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Controls;

[Serializable]
public enum CustomHalftoneMode
{
    Circle,
    Square,
    Line
}

[Title("Custom", "Custom Halftone Node")]
public class CustomHalftoneNode : CodeFunctionNode
{
    public CustomHalftoneNode()
    {
        name = "Custom Halftone Node";
    }

    [SerializeField] private CustomHalftoneMode m_HalftoneMode = CustomHalftoneMode.Circle;

    [EnumControl("Mode")]
    public CustomHalftoneMode halftoneMode
    {
        get { return m_HalftoneMode; }
        set
        {
            if (m_HalftoneMode == value)
                return;
            m_HalftoneMode = value;
            Dirty(ModificationScope.Graph);
        }
    }

    private string GetCurrentModeName()
    {
        return Enum.GetName(typeof(CustomHalftoneMode), m_HalftoneMode);
    }

    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod(string.Format("HalftoneCustom_{0}", GetCurrentModeName()), BindingFlags.Static | BindingFlags.NonPublic);
    }



    private static string HalftoneCustom_Circle(
        [Slot(0, Binding.None)] Vector1 Base,
        [Slot(1, Binding.None)] Vector1 Scale,
        [Slot(2, Binding.WorldSpacePosition)] Vector2 UV,
        [Slot(3, Binding.None, 0.01f, 0, 0, 0)] Vector2 Offset,
        [Slot(4, Binding.None)] out Vector1 Out)
    {
        return @"
{
	float2 Direction = Offset/dot(Offset, Offset);
	float2 Position = fmod(abs(float2(dot(UV, Direction), -UV.x*Direction.y + UV.y*Direction.x)) + 0.5, 1) - 0.5;
	Out = Scale*dot(Position, Position)/(0.25*(1-Base));
	Out = 1-saturate((1 - Out) / fwidth(Out));
}
";
    }


    private static string HalftoneCustom_Square(
        [Slot(0, Binding.None)] Vector1 Base,
        [Slot(1, Binding.None)] Vector1 Scale,
        [Slot(2, Binding.WorldSpacePosition)] Vector2 UV,
        [Slot(3, Binding.None, 0.01f, 0, 0, 0)] Vector2 Offset,
        [Slot(4, Binding.None)] out Vector1 Out)
    {
        return @"
{
	float2 Direction = Offset/dot(Offset, Offset);
	float2 Position = fmod(abs(float2(dot(UV, Direction), -UV.x*Direction.y + UV.y*Direction.x)) + 0.5, 1) - 0.5;
	float Radius = Scale*sqrt(1-Base);
	Out = max(abs(Position.x), abs(Position.y))/Radius;
	Out = 1-saturate((1 - Out) / fwidth(Out));
}
";
    }

    private static string HalftoneCustom_Line(
        [Slot(0, Binding.None)] Vector1 Base,
        [Slot(1, Binding.None)] Vector1 Scale,
        [Slot(2, Binding.None)] Vector1 Width,
        [Slot(3, Binding.WorldSpacePosition)] Vector2 UV,
        [Slot(4, Binding.None, 0.01f, 0, 0, 0)] Vector2 Offset,
        [Slot(5, Binding.None)] out Vector1 Out)
    {
        return @"
{
	float2 Direction = Offset/dot(Offset, Offset);
	float2 Position = fmod(abs(float2(dot(UV, Direction), -UV.x*Direction.y + UV.y*Direction.x)) + 0.5, 1) - 0.5;
	float Radius = Scale*sqrt(1-Base);
	Out = max(abs(Position.x*Width), abs(Position.y))/Radius;
	Out = 1-saturate((1 - Out) / fwidth(Out));
}
";
    }


}