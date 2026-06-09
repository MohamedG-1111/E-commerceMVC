document.addEventListener("DOMContentLoaded", function () {
    initCoverImage();
});

function initCoverImage() {
    const previewImg = document.getElementById("previewImg");
    const previewContainer = document.getElementById("previewContainer");
    const dropZone = document.getElementById("dropZone");
    const fileInput = document.getElementById("Cover");

    if (!previewImg || !previewContainer || !dropZone || !fileInput) return;

    const original = previewImg.getAttribute("data-original");

    // لو في صورة أصلية (Edit Mode)
    if (original && original.trim() !== "") {
        previewImg.src = original;
        previewContainer.classList.remove("d-none");
        dropZone.classList.add("d-none");
    } else {
        previewContainer.classList.add("d-none");
        dropZone.classList.remove("d-none");
    }
}

/* =========================
   HANDLE FILE UPLOAD
========================= */
function handleFile(file) {
    if (!file) return;

    const reader = new FileReader();

    reader.onload = function (e) {
        const previewImg = document.getElementById("previewImg");
        const previewContainer = document.getElementById("previewContainer");
        const dropZone = document.getElementById("dropZone");

        if (!previewImg || !previewContainer || !dropZone) return;

        previewImg.src = e.target.result;

        previewContainer.classList.remove("d-none");
        dropZone.classList.add("d-none");
    };

    reader.readAsDataURL(file);
}

/* =========================
   DRAG & DROP
========================= */
function handleDrop(event) {
    event.preventDefault();

    const fileInput = document.getElementById("Cover");
    const file = event.dataTransfer.files[0];

    if (!fileInput || !file) return;

    fileInput.files = event.dataTransfer.files;

    handleFile(file);
}

/* =========================
   RESET IMAGE (BACK TO ORIGINAL)
========================= */
function resetImage() {
    const previewImg = document.getElementById("previewImg");
    const previewContainer = document.getElementById("previewContainer");
    const dropZone = document.getElementById("dropZone");
    const fileInput = document.getElementById("Cover");

    if (!previewImg || !previewContainer || !dropZone || !fileInput) return;

    const original = previewImg.getAttribute("data-original");

    fileInput.value = "";

    if (original && original.trim() !== "") {
        previewImg.src = original;

        previewContainer.classList.remove("d-none");
        dropZone.classList.add("d-none");
    } else {
        previewContainer.classList.add("d-none");
        dropZone.classList.remove("d-none");
    }
}
tinymce.init({
    selector: 'textarea',
    plugins: [
        'anchor', 'autolink', 'charmap', 'codesample', 'emoticons', 'link',
        'lists', 'media', 'searchreplace', 'table', 'visualblocks', 'wordcount'
    ],
    toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link media table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',
    skin: 'oxide',
    content_css: 'default',
    menubar: false,
    resize: true,
});