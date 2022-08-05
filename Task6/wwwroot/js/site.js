let properties = {
    "Locale": "en_US",
    "ErrorProbability": 0,
    "Seed": "",
    "Page": 0
}

$(document).ready(async function () {
    SetSeed();
    await GenerateData();
});

async function GenerateData() {
    properties.Page++;
    let response = await LoadData();
    var trHTML = '';
    $.each(response, function (i, person) {
        trHTML += '<tr><td>' + person.id + '</td><td>'
            + person.uuid + '</td><td>'
            + person.fullName + '</td><td>'
            + person.address + '</td><td>'
            + person.phone + '</td</tr>';
    });
    $('#persons').append(trHTML);
}

async function LoadData() {
    return await $.ajax({
        url: generateUrl,
        dataType: "json",
        data: properties,
    });
}

$(window).scroll(async function () {
    if ($(document).height() - window.innerHeight == $(this).scrollTop().toFixed()) {
        await GenerateData();
    }
});

$('#locale').on('change', async function () {
    properties.Locale = this.value;
    await ChangeTable();
});

$('#seed').on('input', async function () {
    properties.Seed = this.value;
    await ChangeTable();
});

$('#errorProbability').on('input', async function () {
    if (this.value > 1000) {
        this.value = 1000;
        properties.ErrorProbability = 1000;
        return;
    }else if (this.value <= -1) {
        this.value = 0;
        properties.ErrorProbability = 0;
        return;
    }else if (this.value === '') {
        properties.ErrorProbability = 0;
        $('#errorProbabilitySlider').val(0);
    }else{
        $('#errorProbabilitySlider').val(this.value);
        properties.ErrorProbability = this.value;
    }
    await ChangeTable();
});

$('#errorProbabilitySlider').on('change', async function () {
    properties.ErrorProbability = this.value;
    await ChangeTable();
}).on('input', async function () {
    $('#errorProbability').val(this.value);
    }
);

async function ChangeTable() {
    properties.Page = 0;
    $("#persons").empty();
    await GenerateData();
}

async function ChangeSeed() {
    SetSeed();
    await ChangeTable();
}

function SetSeed() {
    properties.Seed = Math.random().toString(16).substring(2, 10);
    $('#seed').val(properties.Seed);
}

function DownloadCsv() {
    let link = $.param(properties);
    window.location.href = downloadUrl + '?' + link;
}
