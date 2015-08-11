function set_focus_color(obj, set, color) {

    if (color === undefined) {
        var oldColor = '#FAFAD2';   // LightGoldenrodYellow 
        var newColor = '#F9F97C';   // LightGoldenrodYellow - Highlighted
        obj.style.backgroundColor = set ? newColor : oldColor;
    }
    else {
        obj.style.backgroundColor = color;
    }
}