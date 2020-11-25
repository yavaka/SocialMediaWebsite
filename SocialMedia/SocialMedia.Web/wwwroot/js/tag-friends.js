var inputValue;
var taggedUsers = new Array();

window.onload = function () {
    // Used when a post or comment is edited
    if ($('#taggedUsers').val()) {
        var tagged = JSON.parse($('#taggedUsers').val());

        for (var i = 0; i < tagged.length; i++) {
            taggedUsers.push(tagged[i]);
        }
    }
};

// Get user input
$('#userInput').on('input', function () {
    inputValue = $(this).val();
    //Check for the last char of user input
    if (inputValue.charAt(inputValue.length - 1) == '@') {
        showDropDownList();
    }
})

// Show drop down list
function showDropDownList() {
    $('#searchDropdown').toggle();

    $('#userInput').prop("disabled", true);
}

// Hide drop down list when one of the anchor tags is clicked
function hideDropdown(userId) {
    removeDropDownResults();

    // hide the drop down list
    $('#searchDropdown').hide();

    // get the user who will be tagged
    // done() wait for the ajax response
    getUserById(userId);

    $('#userInput').prop("disabled", false);
}

// Hide drop down list on click outside of it
$(document).on('click', function (event) {
    var $trigger = $('#searchDropdown');
    if ($trigger !== event.target && !$trigger.has(event.target).length) {
        $('#searchDropdown').slideUp('fast');
    }
    document.getElementById('searchInput').value = "";
    removeDropDownResults();

    $('#userInput').prop("disabled", false);
});

// Get users who consists part name in their names
function getUsersByPartName() {
    var searchInput = document.getElementById('searchInput').value;
    removeDropDownResults();
    if (searchInput.length >= 3) {
        $.ajax({
            type: 'GET',
            url: '/Friendships/GetUserFriendsByPartName',
            data: { 'partName': searchInput },
            contentType: 'application/json',
            dataType: 'json',
            success: function (data) {
                for (var i = 0; i < data.length; i++) {
                    let anchorTag = "<a onclick=\"hideDropdown(\'" + data[i].id + "\')\" style=\"cursor: pointer\">" + data[i].fullName + "(" + data[i].userName.trim() + ")</a>";
                    $('#results').append(anchorTag);
                }
            }
        });
    }
}

// Get user by id
function getUserById(friendId) {
    $.post({
        url: '/Friendships/GetFriendById',
        data: {
            friendId: friendId
        },
        success: function (res) {
            // add the tagged user in collection
            taggedUsers.push(res);
            // display the tagged user`s username
            displayTaggedUser(res.userName.trim());
        },
        error: function (msg) {
            console.log(msg);
        }
    });
}

// Display the tagged user in the input field
function displayTaggedUser(userName) {
    var oldInput = $('#userInput').val();
    var newInput = oldInput.concat(userName);
    $('#userInput').val(newInput);
}

// Catch every press of delete and backspace buttons
$('#userInput').on('keydown', function (e) {
    // if there are tagged users get into the body
    if (taggedUsers.length > 0) {
        //get the pressed key code or char code
        var key = e.keyCode || e.charCode;
        //if pressed key is delete or backspace
        if (key == 8 || key == 46) {
            //Get input tag text value
            inputValue = $('#userInput').val();
            if (inputValue != '' &&
                inputValue != ' ' &&
                inputValue != null &&
                inputValue != undefined) {
                // get the whole word where the caret is
                var word = getWord();
                if (word[0] == '@') {
                    inputValue = inputValue.replace(word, '');
                    $('#userInput').val(inputValue);
                    removeTaggedUser(word);
                }
            }
        }
    }
});

// Get caret/cursor position
function getCaret(node) {
    if (node.selectionStart) {
        return node.selectionStart;
    } else if (!document.selection) {
        return 0;
    }
    var c = '\001',
        sel = document.selection.createRange(),
        dul = sel.duplicate(),
        len = 0;
    dul.moveToElementText(node);
    sel.text = c;
    len = dul.text.indexOf(c);
    sel.moveStart('character', -1);
    sel.text = '';
    return len;
}

// Get the word where the cursor position is in the input field
function getWord() {
    var el = document.getElementById('userInput');
    var carret = getCaret(el);
    var words = el.value.split(' ');
    var x = 0;
    for (var i = 0; i < words.length; i++) {
        x += words[i].length + 1;
        if (x > carret) {
            return words[i];
        }
    }
}

function removeDropDownResults() {
    var dropDownResults = document.getElementById('results');
    var resultsChildren = dropDownResults.childNodes;
    for (var i = 0; i < resultsChildren.length; i++) {
        dropDownResults.removeChild(resultsChildren[i]);
    }
}

function assignTaggedUsers() {
    if (taggedUsers.length > 0) {
        var taggedUsersAsJson = JSON.stringify(taggedUsers);
        $('#taggedUsers').val(taggedUsersAsJson);
    }
}

function removeTaggedUser(taggedUser) {

    taggedUser = taggedUser.substring(1);

    for (var i = 0; i < taggedUsers.length; i++) {
        var thisUser = taggedUsers[i];
        if (thisUser !== undefined) {
            var username;
            // due to different json serializations user name can be .userName or .UserName :)
            if (thisUser.userName === undefined) {
                username = thisUser.UserName.trim();
            }
            else {
                username = thisUser.userName.trim();
            }
            if (taggedUser === username) {
                delete taggedUsers[i];
                break;
            }
        }
    }
}