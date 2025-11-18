$(document).ready(function () {
  // Khởi tạo DataTable và lưu vào biến table
  var table = $("#customerDatatable").DataTable({
    processing: true,
    serverSide: true,
    filter: true,
    ajax: {
      url: "/api/VendorApi",
      type: "GET",
      datatype: "json",
      dataSrc: "data",
    },
    columnDefs: [
      {
        targets: [0],
        visible: false,
        searchable: false,
      },
    ],
    columns: [
      { data: "id", name: "Id", autoWidth: true },

      {
        targets: 1,
        width: "80px",
        orderable: false,
        render: function (data, type, row) {
          var Id = "";
          if (type === "display" && data !== null) {
            Id = row.id;
          }
          return `<a href="/Vendor/Edit/${Id}" m-1">View | Edit</a>`;
        },
      },

      {
        data: null,
        name: "STT1",
        width: "50px",
        autoWidth: true,
        orderable: false,
        searchable: false,
        render: function (data, type, row, meta) {
          return meta.row + meta.settings._iDisplayStart + 1;
        },
      },

      { data: "vendorCode", name: "vendorCode", autoWidth: true },
      { data: "vendorName", name: "vendorName", autoWidth: true },
      { data: "phone", name: "phone", autoWidth: true, orderable: false },
      { data: "symbol", name: "symbol", autoWidth: true, orderable: false },

      {
        data: "status",
        name: "status",
        autoWidth: true,
        orderable: false,
        render: function (data, type, row) {
          if (data === 0) {
            return '<span class="badge badge-success">Active</span>';
          } else if (data === 1) {
            return '<span class="badge badge-danger">Off</span>';
          } else {
            return '<span class="badge badge-secondary">Unknown</span>';
          }
        },
      },

      // Cột icon xoá ở cuối
      {
        data: null,
        orderable: false,
        searchable: false,
        width: "40px",
        render: function (data, type, row) {
          return `
                        <a href="javascript:void(0);" 
                           class="text-danger btn-delete" 
                           data-id="${row.id}" 
                           title="Xoá">
                            <i class="fa fa-trash"></i>
                        </a>`;
        },
      },
    ],
    lengthMenu: [
      [5, 10, 20, 50, 100],
      [5, 10, 20, 50, 100],
    ],
    pageLength: 5,
  });

  // ⭐ BẮT SỰ KIỆN CLICK NÚT XOÁ
  $("#customerDatatable").on("click", ".btn-delete", function () {
    var id = $(this).data("id");

    if (!confirm("Bạn có chắc muốn xoá nhà cung cấp này?")) {
      return;
    }

    $.ajax({
      url: "/Vendor/DeleteAjax", // gọi đúng action trong VendorController
      type: "POST",
      data: { id: id },
      success: function (res) {
        if (res.success) {
          // reload lại DataTable, giữ nguyên trang hiện tại
          table.ajax.reload(null, false);

          // dùng toast nếu đã khai báo, chưa có thì dùng alert
          if (typeof showToast === "function") {
            showToast(res.message || "Xoá thành công");
          } else {
            alert(res.message || "Xoá thành công");
          }
        } else {
          if (typeof showToast === "function") {
            showToast(res.message || "Xoá thất bại", "error");
          } else {
            alert(res.message || "Xoá thất bại");
          }
        }
      },
      error: function () {
        if (typeof showToast === "function") {
          showToast("Có lỗi xảy ra khi xoá.", "error");
        } else {
          alert("Có lỗi xảy ra khi xoá.");
        }
      },
    });
  });
});
