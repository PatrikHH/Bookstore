$(document).ready(function () {
    $('.delete-store').on('click', function () {
        var id = $(this).data('id');
        var url = $(this).data('url');  

        Swal.fire({
            title: 'Are u sure?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#dc3545',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Delete',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: url,
                    type: 'POST',
                    data: { id: id },
                    success: function (response) {
                        if (response.success)
                            Swal.fire({
                                position: "top-end",
                                title: 'Deleted',
                                icon: 'success',
                                text: response.message,
                                showConfirmButton: false,
                                timer: 1500
                            }).then(() => {
                                location.reload();
                            });
                        else
                            Swal.fire({
                                title: 'Error!',
                                icon: 'error',
                                text: response.message
                            });
                    },
                    error: function () {
                        Swal.fire('Error!', 'Some error.', 'error');
                    }
                });
            }
        });
    });
});
