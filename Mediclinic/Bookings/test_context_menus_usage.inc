
<div id="ctxt_menu_examples" runat="server">

    <span class="demo1" id="quickDemo" style="float:right;border: 1px solid #888;">
        <b>DEMO</b> right-click me!!
    </span>


    <h3>Example 1 - Basic usage with bindings</h3>
    <span class="demo1" id="demo1_yellow">
        <b>RIGHT CLICK FOR DEMO</b>
    </span>
    <span class="demo1" id="demo1_green" style="background-color:lightgreen">
        <b>THIS WORKS TOO</b>
    </span>



    <h3>Example 2 - Basic styling </h3>
    <p id="demo2" style="background-color:yellow;">
        Right clicking anywhere in this paragraph will trigger the context menu.
    </p>

    <h3>Example 3 - Advanced usage with callbacks</h3>
    <p style="font-weight: bold;">
        <span class="demo3" id="dontShow" style="padding: 5px; background-color:pink">Don't show menu</span>
        <span class="demo3" id="showOne" style="padding: 5px;background-color:lightgreen">Just first item</span>
        <span class="demo3" id="showAll" style="padding: 5px;background-color:lightblue">Show all</span>
    </p>

    <hr />

    <table>
        <tr>
            <th>Name</th>
            <th>Short</th>
        </tr>
        <tr>
            <td class="showMyContext" id="td1">name1</td>
            <td>myname1</td>
        </tr>
        <tr>
            <td class="showMyContext" id="td2">name2</td>
            <td>myname2</td>
        </tr>
    </table>


    <table>
        <tr>
            <th>Name</th>
            <th>Short</th>
        </tr>
        <tr>
            <td class="showEditableContext" id="1">name1</td>
            <td>myname1</td>
        </tr>
        <tr>
            <td class="showEditableContext" id="2">name2</td>
            <td>myname2</td>
        </tr>
    </table>

    <hr />

</div>
