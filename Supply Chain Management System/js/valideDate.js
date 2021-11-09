function formatdate(elementId) {
             var fdt;
             
             var dt= document.getElementById(elementId).value;
             var day;
             var mon;
             var yr;
             if (dt.length > 0) {
                 dt = dt.toUpperCase();
                 dt = dt.replace('/', '').replace('/', '').replace('.', '').replace('.', '').replace('-', '').replace('-', '');
                 dt = dt.replace('JAN', '01').replace('FEB', '02').replace('MAR', '03').replace('APR', '04').replace('MAY', '05').replace('JUN', '06').replace('JUL', '07').replace('AUG', '08').replace('SEP', '09').replace('OCT', '10').replace('NOV', '11').replace('DEC', '12');
                 day = dt.substr(0, 2);
                 mon = dt.substr(2, 2);
                 yr = dt.substr(4);
                 if (yr.length == 2) {
                     if (yr>30)
                     {   
                     yr = '19' + yr;
                     }
                     else
                     {
                     yr = '20' + yr;
                     }
                 }
                 if ((dt.length < 6) || (dt.length > 10)) {
                     alert("Invalid Date\nPlease Re-Enter");
                     document.getElementById(elementId).focus();
                 }
                 else if (parseInt(day) >= 32) {
                     alert("Invalid Date\nPlease Re-Enter");
                     document.getElementById(elementId).focus();
                 }

                 else if (parseInt(mon) > 12) {
                     alert("Invalid Date\nPlease Re-Enter");
                     document.getElementById(elementId).focus();
                 }
                 else {
                     fdt = day + '/' + mon + '/' + yr;
                     document.getElementById(elementId).value = fdt;
                 }
             }             
            } 