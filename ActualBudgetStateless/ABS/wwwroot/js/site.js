$(function() {
    $('[data-href]').on('click', function() {
        window.location = $(this).data('href');
    });

    $('[data-toggle="tooltip"]').tooltip();
});