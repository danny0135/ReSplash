// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



$("#photoModal").on('show.bs.modal', function (event) {
    // Image that was clicked
    const btnImage = event.relatedTarget

    // Get the photo id from data-bs-* attribute
    const photoId = btnImage.getAttribute('data-bs-photo')

    const apiUrl = 'photo/get/' + photoId

    $.get(apiUrl, function (photo) {

        // Switch the image
        const photoUrl = '/photos/' + photo.fileName
        $('#modal-image').attr('src', photoUrl)

        // set values for other fields
        $('#modal-createdby').text('Created by: ' + photo.createdBy)

        $('#modal-description').text(photo.description)

        $('#modal-category').text('Category: ' + photo.category)

        const tags = photo.tags
        const strTags = tags.join(', ')
        $('#modal-tags').text('Tags: ' + strTags)
    })
})