var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "city", "width": "15%" },
            { "data": "role", "width": "10%" },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `
                        <div style="display:flex; justify-content:center;">
                             <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:50%;">
                                <i class="bi bi-lock-fill"></i>Lock
                            </a> 
                            <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-warning text-white" style="cursor:pointer; width:50%; margin-left: 2px">
                                <i class="bi bi-person-square"></i>Permission
                            </a>
                        </div>
                    `
                    }
                    else {
                        return `
                        <div style="display:flex; justify-content:center;">
                              <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:50%;">
                                    <i class="bi bi-unlock-fill"></i>UnLock
                                </a>
                                <a href="/admin/user/RoleManagement?userId=${data.id}" class="btn btn-warning text-white" style="cursor:pointer; width:50%; margin-left: 2px">
                                     <i class="bi bi-person-square"></i>Permission
                                </a>
                        </div>
                    `
                    }
                },
                "width": "30%"
            }
        ]
    });
}


function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}