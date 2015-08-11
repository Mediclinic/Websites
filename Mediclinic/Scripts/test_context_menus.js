        $(document).ready(function () {

            $('span.demo1').contextMenu('myMenu1', {
                bindings: {
                    'open': function (t) {
                        alert('Trigger was ' + t.id + '\nAction was Open');
                    },
                    'email': function (t) {
                        alert('Trigger was ' + t.id + '\nAction was Email');
                    },
                    'save': function (t) {
                        alert('Trigger was ' + t.id + '\nAction was Save');
                    },
                    'delete': function (t) {
                        alert('Trigger was ' + t.id + '\nAction was Delete');
                    }
                }
            });

            $('#demo2').contextMenu('myMenu2', {
                menuStyle: {
                    border: '2px solid #000'
                },
                itemStyle: {
                    fontFamily: 'verdana',
                    backgroundColor: '#666',
                    color: 'white',
                    border: 'none',
                    padding: '1px'
                },
                itemHoverStyle: {
                    color: '#fff',
                    backgroundColor: '#0f0',
                    border: 'none'
                }
            });

            $('span.demo3').contextMenu('myMenu3', {
                onContextMenu: function (e) {
                    if ($(e.target).attr('id') == 'dontShow') return false;
                    else return true;
                },
                onShowMenu: function (e, menu) {
                    if ($(e.target).attr('id') == 'showOne') {
                        $('#item_2, #item_3', menu).remove();
                    }
                    return menu;
                }
            });
        });


          $(document).ready(function () {
            $('td.showMyContext').contextMenu('myMenuA', {
 
              bindings: {
                'email': function (t) {
                  document.location.href = '/contact/sendmail?id=' + t.id;
                },
                'homepage': function (t) {
                  document.location.href = '/contact/homepage?id=' + t.id;
                }
              }
            });
          });

  $(document).ready(function () {
      $('td.showMyContext2').contextMenu('myMenuA', {

          bindings: {
              'email': function (t) {
                  document.location.href = '/contact/sendmail?idx=' + t.id;
              },
              'homepage': function (t) {
                  document.location.href = '/contact/homepage?idx=' + t.id;
              }
          }
      });
  });
