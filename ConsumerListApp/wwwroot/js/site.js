$(document).ready(function () {
    $('.message a').on('click', function () {
        $('form').animate({
            height: "toggle",
            opacity: "toggle"
        }, "fast");
    });
    divsO = document.getElementsByClassName('show-hide');
});

function SearchPara() {
    var data = {
        Flag: $('#Flag').val(),
        FeederInput: $('#FeederInput').val(),
        SubstaionCodeInput: $('#SubstaionCodeInput').val(),
        SubDivisionCodeInput: $('#SubDivisionCodeInput').val(),
    };

    return data;
}

$('#search-btn').click(function () {
    AdminTable();
    TableLoad();
    document.getElementById("save-btn").style.display = 'block';
    $('#save-btn').val('Save Data');
    $('#save-btn').css("background-color", "#007BFF");
    $('#save-btn').attr("disabled", false);
})

$('#save-btn').click(function () {
    $.ajax({
        url: '/Home/SaveToDb',
        type: 'POST',
        data: SearchPara(),
        success: function () {
            console.log("success");
        },
        error: function (err) {
            console.log(err);
        }
    });
    $('#save-btn').val('Saved Successfully');
    $('#save-btn').css("background-color", "#04AA6D");
    $('#save-btn').attr("disabled", true);

})

function showHide(elem) {
    if (elem.selectedIndex != 0) {
        document.getElementById("default-view").style.display = 'none';
        for (var i = 0; i < divsO.length; i++) {
            divsO[i].style.display = 'none';
        }
        document.getElementById(elem.value).style.display = 'block';
        document.getElementById("search-btn").style.display = 'block';
        document.getElementById("modal-grid-btn").style.display = 'block';
    }
}

function AdminTable() {
    $('#ConsumerTableContainer').jtable({
        title: 'Response Data',
        paging: true,
        pageSize: 10,
        sorting: false,
        actions:
        {
            listAction: '/Home/GetConsumerData'
        },
        fields: {
            name: {
                title: 'Name',
                list: true,
                Width: '10%',
            },
            mobile_no: {
                title: 'MobileNumber',
                list: true,
                Width: '10%',
            },
            substation_code: {
                title: "SubstationCode",
                list: true,
                Width: '10%',
            },
            feeder_code: {
                title: 'FeederCode',
                Width: '10%',
            },
            feeder_name: {
                title: 'FeederName',
                Width: '10%',
            },
            accno: {
                title: 'AccountNumber',
                Width: '10%',
            },
            sdocode: {
                title: 'SdoCode',
                Width: '10%',
            },
            address: {
                title: 'Address',
                Width: '20%',
            }
            //Address: {
            //    title: 'Address',
            //    Width: '10%',
            //    display: function (data) {
            //        return '<a href="#show-address" style="text-decoration: none; font-size: 12px;" onclick="showAddressModal(\'' + data.record.address + '\')">Show Address</a>';
            //    },
            //},
        }
    });    
}

function showAddressModal(address) {
    $("#showAddressId").text("Full Address:- " + address);
}

function TableLoad() {
    $('#ConsumerTableContainer').jtable('load', SearchPara());
}