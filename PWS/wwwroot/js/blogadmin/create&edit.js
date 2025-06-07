// A jank ass way to pass razor values to script
var summaryCount;
if (SummaryCharacterCount != null) {
    summaryCount = SummaryCharacterCount;
}

const quillEditorOptions = {
    // debug: 'info',
    modules: {
        toolbar: true,
    },
    placeholder: 'Time to talk whisky!',
    theme: 'snow'
};

quill = new Quill('#editor_0', quillEditorOptions); // Setup quill

// set Content based on hidden content field
if (document.getElementById("Content").value != "") {
    quill.setContents(JSON.parse(JSON.parse(document.getElementById("Content").value)));
}

// On text change, set the toSave input
quill.on('text-change', (delta, oldDelta, source) => {

    if (quill.getLength() <= 1) {
        document.getElementById("Content").value = "";
        document.getElementById("Summary").value = "";
        return;
    }

    var content = JSON.stringify(JSON.stringify(quill.getContents()));

    document.getElementById("Content").value = content;

    var summary = quill.getText(0, summaryCount); //JSON.stringify(JSON.stringify(quill.getContents(0, @Blog.SummaryCharacterCount)));

    document.getElementById("Summary").value = summary;
});
quill.enable() //to enable editor