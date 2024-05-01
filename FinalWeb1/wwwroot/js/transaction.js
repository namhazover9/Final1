$(document).ready(function () {
    $('#myTable').DataTable({
        "scrollY": "450px",
        "scrollCollapse": true,
        "paging": true
    });
});

/*var dataTable;*/

//$(document).ready(function () {
//    $('#tblData').DataTable();
//});
//$(document).ready(function () {
//    loadDataTable();
//});

//function loadDataTable() {
//    dataTable = $('#tblData').DataTable({
//        "ajax": { url: '/admin/transaction/getall' },
//        "columns": [
//            { data: 'id', "width": "5%" },
//            { data: 'name', "width": "20%" },
//            { data: 'applicationUser.id', "width": "20%" },
//            { data: 'applicationUser.name', "width": "20%" },
//            { data: 'status', "width": "10%" },
//            {
//                data: 'id',
//                "render": function (data) {
//                    return `<div class="w-75 btn-group" role="group">
//                     <a href="/admin/transaction/summary?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Summary</a>                                
//                    </div>`
//                },
//                "width": "25%"
//            }
//        ]
//    });
//}
//function loadDataTable() {
//    dataTable = $('#tblData').DataTable({
//        "ajax": { url: '/admin/transaction/getall' },
//        "columns": [
//            { data: 'id', "width": "5%" },
//            { data: 'name', "width": "20%" },
//            { data: 'applicationUser.id', "width": "30%" },
//            { data: 'applicationUser.name', "width": "20%" },
//            { data: 'status', "width": "10%" },
//            {
//                data: 'id',
//                "render": function (data) {
//                    return `<div class="w-75 btn-group" role="group">
//                     <a href="/Admin/Transaction/Summary?id=${data}> Summary</a>
//                     <a href="/Admin/Transaction/Details/${data}">Summary</a>
//                    </div>`
//                },
//                "width": "15%"
//            }


            
//        ]
//    });
//}

//function Delete(url) {
//    Swal.fire({
//        title: 'Are you sure?',
//        text: "You won't be able to revert this!",
//        icon: 'warning',
//        showCancelButton: true,
//        confirmButtonColor: '#3085d6',
//        cancelButtonColor: '#d33',
//        confirmButtonText: 'Yes, delete it!'
//    }).then((result) => {
//        if (result.isConfirmed) {
//            $.ajax({
//                url: url,
//                type: 'DELETE',
//                success: function (data) {
//                    dataTable.ajax.reload();
//                    toastr.success(data.message);
//                }
//            })
//        }
//    })
//}