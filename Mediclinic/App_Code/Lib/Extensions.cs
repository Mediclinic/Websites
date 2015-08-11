using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

//myObject.AddCssClass("someclass");
//myObject.RemoveCssClass("someclass");

public static class WebControlExtensions
{
    public static void AddCssClass(this HtmlGenericControl control, string cssClass)
    {
        List<string> classes;
        if (!string.IsNullOrWhiteSpace(control.Attributes["class"]))
        {
            classes = control.Attributes["class"].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!classes.Contains(cssClass))
                classes.Add(cssClass);
        }
        else
        {
            classes = new List<string> { cssClass };
        }
        control.Attributes["class"] = string.Join(" ", classes.ToArray());
    }

    public static void RemoveCssClass(this HtmlGenericControl control, string cssClass)
    {
        List<string> classes = new List<string>();
        if (!string.IsNullOrWhiteSpace(control.Attributes["class"]))
        {
            classes = control.Attributes["class"].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        classes.Remove(cssClass);
        control.Attributes["class"] = string.Join(" ", classes.ToArray());
    }

    public static void AddCssClass(this HtmlControl control, string cssClass)
    {
        List<string> classes;
        if (!string.IsNullOrWhiteSpace(control.Attributes["class"]))
        {
            classes = control.Attributes["class"].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!classes.Contains(cssClass))
                classes.Add(cssClass);
        }
        else
        {
            classes = new List<string> { cssClass };
        }
        control.Attributes["class"] = string.Join(" ", classes.ToArray());
    }

    public static void RemoveCssClass(this HtmlControl control, string cssClass)
    {
        List<string> classes = new List<string>();
        if (!string.IsNullOrWhiteSpace(control.Attributes["class"]))
        {
            classes = control.Attributes["class"].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        classes.Remove(cssClass);
        control.Attributes["class"] = string.Join(" ", classes.ToArray());
    }

    public static void AddCssClass(this WebControl control, string cssClass)
    {
        List<string> classes;
        if (!string.IsNullOrWhiteSpace(control.CssClass))
        {
            classes = control.CssClass.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (!classes.Contains(cssClass))
                classes.Add(cssClass);
        }
        else
        {
            classes = new List<string> { cssClass };
        }
        control.CssClass = string.Join(" ", classes.ToArray());
    }

    public static void RemoveCssClass(this WebControl control, string cssClass)
    {
        List<string> classes = new List<string>();
        if (!string.IsNullOrWhiteSpace(control.CssClass))
        {
            classes = control.CssClass.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        classes.Remove(cssClass);
        control.CssClass = string.Join(" ", classes.ToArray());
    }
}
