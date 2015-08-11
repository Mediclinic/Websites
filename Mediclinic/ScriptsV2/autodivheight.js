
var run_timer; // global variable that u put in a timer to run after 200ms (see in the methods for more details)

window.onresize = function (event) {
    clearTimeout(run_timer);
    run_timer = setTimeout(update_to_max_height, 200);  // wait 200ms after resizing of window so that if they are still resizing it, it waits until they have stopped resizing it
}

/*
window.onload = function () {
    update_to_max_height();
};
*/

addLoadEvent(function () { update_to_max_height(); });

String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

function update_to_max_height() {  // update height of sheet so window just barely has no vertical scroll

    var controlID = "autodivheight";
    var otherControlID = 'autodivheight_otherconrol';

    if (document.getElementById(controlID) == null)
        return;

    var curText = document.getElementById(controlID).style.height;
    var curVal = parseInt(curText.endsWith("px") ? curText.substring(0, curText.length - 2) : curText);
    var curTextEnding = curText.endsWith("px") ? curText.substring(curText.length - 2) : "";

    var newVal = curVal;

    // if there is a control with this id that is making the page have scroll bars and 
    // ruining the autodivheight code, hide it, re-adjust, then unhide it
    if (document.getElementById(otherControlID) != null)
        document.getElementById(otherControlID).style.display = "none";

    newVal = resize_vertical(false, newVal, 256, curTextEnding, controlID); // decrease the height by 256 until there is no browser scroll showing
    newVal = resize_vertical(true,  newVal, 64,  curTextEnding, controlID); // increase the height by 64  until there is a  browser scroll showing
    newVal = resize_vertical(false, newVal, 16,  curTextEnding, controlID); // decrease the height by 16  until there is no browser scroll showing
    newVal = resize_vertical(true,  newVal, 4,   curTextEnding, controlID); // increase the height by 4   until there is a  browser scroll showing

    if (document.getElementById(otherControlID) != null)
        document.getElementById(otherControlID).style.display = "";
}

function resize_vertical(whileHasScroll, newVal, stepSize, curTextEnding, controlID) {

    var hasVScroll = document.documentElement.scrollHeight > document.documentElement.clientHeight;

    if (whileHasScroll) {
        while ((newVal - stepSize) > 0 && hasVScroll) {
            newVal -= stepSize;
            document.getElementById(controlID).style.height = newVal + curTextEnding;
            hasVScroll = document.documentElement.scrollHeight > document.documentElement.clientHeight;
        }
    }
    else {
        while (!hasVScroll) {
            newVal += stepSize;
            document.getElementById(controlID).style.height = newVal + curTextEnding;
            hasVScroll = document.documentElement.scrollHeight > document.documentElement.clientHeight;
        }
    }

    return newVal;
}

