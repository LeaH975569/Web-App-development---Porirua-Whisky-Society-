

// This controls the visability of the score fields based on the select on the create and edit page
let defaultDate = "0001-01-01"


let selector = document.getElementById("WhiskeyScoreSetting");
let scoreFinish = document.getElementById("WhiskeyFinish");
let scoreAroma = document.getElementById("WhiskeyAroma");
let scoreTaste = document.getElementById("WhiskeyTaste");
let scoreTotal = document.getElementById("TotalScore");
let tastedDate = document.getElementById("TastedDate");

// Update score field display based on current score setting
updateScoreInputDisplay();

// Set intial values
scoreFinish.value = 0;
scoreAroma.value = 0;
scoreTaste.value = 0;
scoreTotal.value = 0;

// Events
selector.addEventListener('change', updateScoreInputDisplay);

scoreFinish.addEventListener('input', updateTotalScore);
scoreAroma.addEventListener('input', updateTotalScore);
scoreTaste.addEventListener('input', updateTotalScore);

function updateScoreInputDisplay() {
    let selectedValue = selector.value;

    if (selectedValue == "" || selectedValue == -1) {
        // Non selected or not scored
        scoreFinish.parentElement.style.display = "none";
        scoreAroma.parentElement.style.display = "none";
        scoreTaste.parentElement.style.display = "none";
        scoreTotal.parentElement.style.display = "none";
        tastedDate.parentElement.style.display = "none";

        //tastedDate.value = defaultDate
        return;
    }

    if (selectedValue == 0) {
        // ManualTotal
        scoreFinish.parentElement.style.display = "none";
        scoreAroma.parentElement.style.display = "none";
        scoreTaste.parentElement.style.display = "none";
        scoreTotal.parentElement.style.display = "";
        scoreTotal.disabled = false;

        tastedDate.parentElement.style.display = "";

        //if (tastedDate.value == defaultDate)
        //    tastedDate.value == ""
        return;
    }

    if (selectedValue == 1) {
        // ManualSub
        scoreFinish.parentElement.style.display = "";
        scoreAroma.parentElement.style.display = "";
        scoreTaste.parentElement.style.display = "";
        scoreTotal.parentElement.style.display = "";
        scoreTotal.disabled = true;

        tastedDate.parentElement.style.display = "";

        //if (tastedDate.value == defaultDate)
        //    tastedDate.value == ""
        return;
    }

    if (selectedValue == 2) {
        // SurveyResults
        scoreFinish.parentElement.style.display = "none";
        scoreAroma.parentElement.style.display = "none";
        scoreTaste.parentElement.style.display = "none";
        scoreTotal.parentElement.style.display = "none";
        tastedDate.parentElement.style.display = "none";
    }


}

function updateTotalScore(){
    let selectedValue = selector.value;
    if (selectedValue != 1) {
            return;
    }

    let score = ( Number(scoreFinish.value) + Number(scoreAroma.value) + Number(scoreTaste.value) ) / 3;

    scoreTotal.value = score;
}