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

$('#modal-grid-btn').click(function () {
    $.ajax({
        url: '/Home/Grid',
        type: 'POST',
        dataType: 'json',
        data: SearchPara(),
        success: function (data) {
            buildGrid(data);
        },
        error: function (err) {
            console.log(err);
        }
    });
})

var pageContentList = [];
var activePage = 1;

function buildGrid(data) {
    let tableBody = document.getElementById('grid-body');
    tableBody.innerHTML = '';  // Clear the existing table body
      
    let pageBody = document.getElementById('page-list');
    if (pageBody != null) {
        pageBody.innerHTML = '';  // Clear the existing page body
    }

    const totalNumberOfPages = Math.ceil(data.length / 10);
        
    for (var i = 0; i < totalNumberOfPages; i++) {
        let pageBody = document.getElementById('page-list');        
        let button = document.createElement('BUTTON');
        button.textContent = i + 1;
        button.value = i + 1;
        button.className = (i + 1) + "-button";
        button.id = 'page';
        pageBody.appendChild(button);

        var currentPageContent = [];

        for (var j = i; j < i+10; j++) {
            let row = document.createElement('tr');

            let columns = ['fullname', 'mobile_no', 'substation_code', 'feeder_code', 'feeder_name', 'accno', 'sdocode', 'address'];
            columns.forEach(column => {
                let cell = document.createElement('td');
                cell.textContent = data[j][column];
                row.appendChild(cell);
            });

            currentPageContent.push(row);
        }

        pageContentList.push(currentPageContent);
    }

    loadGridByPage(1);
    changeActiveButton(1);
}

$(document).on("click", "#page", function () {
    var pageNumber = $(this).val();
    loadGridByPage(pageNumber);
    changeActiveButton(pageNumber);
    //console.log(activePage);
})

$('#prev-btn').click(function () {
    if (activePage > 1) {        
        changeActiveButton(activePage - 1);
        loadGridByPage(activePage);
    }
    //console.log(activePage);
})

$('#next-btn').click(function () {
    
    if (activePage < pageContentList.length) {
        changeActiveButton(activePage + 1);
        loadGridByPage(activePage);
    }
    //console.log(activePage);
})

function loadGridByPage(pageNumber) {
    let tableBody = document.getElementById('grid-body');
    tableBody.innerHTML = '';  // Clear the existing table body
    pageContentList[pageNumber - 1].forEach(row => {
        tableBody.appendChild(row);
    })
}

function changeActiveButton(pageNumber) {
    $('.' + activePage + '-button').css("backgroundColor", "");
    $('.' + activePage + '-button').css("color", "");
    $('.' + pageNumber + '-button').css("color", "white");
    $('.' + pageNumber + '-button').css("backgroundColor", "#0d6efd");
    activePage = pageNumber;
}

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
            fullname: {
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