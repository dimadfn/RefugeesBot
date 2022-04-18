<?
ifAnswerInFile('Hello!');

function ifAnswerInFile($msg) {
    $msg = strtolower($msg);

    $templates = json_decode(file_get_contents('templates.json'));
    var_dump($templates);
}

?>