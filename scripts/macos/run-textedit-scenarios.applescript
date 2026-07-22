on run argv
    if (count of argv) is 0 then error "An artifacts directory argument is required."

    set artifactsRoot to item 1 of argv
    my ensureDirectory(artifactsRoot)

    set scenarioResults to {}
    set end of scenarioResults to my runScenarioA(artifactsRoot)
    set end of scenarioResults to my runScenarioB()
    set end of scenarioResults to my runScenarioC(artifactsRoot)
    set end of scenarioResults to my runScenarioD(artifactsRoot)
    set end of scenarioResults to my runScenarioE()

    return my joinLines(scenarioResults)
end run


on runScenarioA(artifactsRoot)
    set startedAt to current date
    set savePath to artifactsRoot & "/scenario-a-round-trip.txt"
    set expectedText to my buildLoremIpsumDocument(550)

    try
        my resetTextEdit()
        my createPlainDocument(expectedText)
        my saveFrontDocument(savePath)
        my closeFrontDocumentWithoutSaving()
        my openDocument(savePath)

        set actualText to my readFrontDocumentContents()
        my closeFrontDocumentWithoutSaving()

        if actualText is expectedText then
            return my formatResult("ScenarioA", "passed", startedAt, "Round-trip content matched exactly.")
        end if

        return my formatResult("ScenarioA", "failed", startedAt, "Reopened TextEdit content did not match the original text.")
    on error errorMessage
        return my formatResult("ScenarioA", "failed", startedAt, errorMessage)
    end try
end runScenarioA


on runScenarioB()
    set startedAt to current date
    return my formatResult("ScenarioB", "skipped", startedAt, "Planned: TextEdit find-and-replace dialog mapping has not been implemented yet.")
end runScenarioB


on runScenarioC(artifactsRoot)
    set startedAt to current date
    set savePath to artifactsRoot & "/scenario-c-unicode.txt"
    set expectedText to "Emoji line: 😀" & linefeed & "Arabic line: مرحبا بكم" & linefeed & "Chinese line: 你好，世界"

    try
        my resetTextEdit()
        my createPlainDocument(expectedText)
        my saveFrontDocument(savePath)

        set encodingReport to do shell script "file -I " & quoted form of savePath

        my closeFrontDocumentWithoutSaving()
        my openDocument(savePath)

        set actualText to my readFrontDocumentContents()
        my closeFrontDocumentWithoutSaving()

        if actualText is expectedText and encodingReport contains "utf-8" then
            return my formatResult("ScenarioC", "passed", startedAt, "Unicode content persisted and the saved file reported utf-8 encoding.")
        end if

        return my formatResult("ScenarioC", "failed", startedAt, "Unicode verification or utf-8 encoding check did not pass.")
    on error errorMessage
        return my formatResult("ScenarioC", "failed", startedAt, errorMessage)
    end try
end runScenarioC


on runScenarioD(artifactsRoot)
    set startedAt to current date
    set savePath to artifactsRoot & "/scenario-d-unsaved.txt"
    set originalText to "Original saved file content."
    set modifiedText to "Modified content that must remain unsaved after cancel."

    try
        do shell script "printf %s " & quoted form of originalText & " > " & quoted form of savePath

        my resetTextEdit()
        my openDocument(savePath)
        my setFrontDocumentContents(modifiedText)
        my requestCloseFrontDocument()

        delay 0.5

        tell application "System Events"
            tell process "TextEdit"
                set frontmost to true

                if not (exists sheet 1 of window 1) then
                    error "The unsaved-changes confirmation sheet did not appear."
                end if

                click button "Cancel" of sheet 1 of window 1
            end tell
        end tell

        delay 0.5

        set actualEditorText to my readFrontDocumentContents()
        set diskText to do shell script "cat " & quoted form of savePath
        set appStillOpen to my checkTextEditRunning()

        my closeFrontDocumentWithoutSaving()

        if appStillOpen and actualEditorText is modifiedText and diskText is originalText then
            return my formatResult("ScenarioD", "passed", startedAt, "Cancel kept TextEdit open and preserved the unsaved editor state.")
        end if

        return my formatResult("ScenarioD", "failed", startedAt, "Cancel did not preserve the expected TextEdit editor state and on-disk file content.")
    on error errorMessage
        return my formatResult("ScenarioD", "failed", startedAt, errorMessage)
    end try
end runScenarioD


on runScenarioE()
    set startedAt to current date
    set originalText to "baseline text"
    set replacementText to "replacement"

    try
        my resetTextEdit()
        my createPlainDocument(originalText)

        tell application "System Events"
            tell process "TextEdit"
                set frontmost to true
                keystroke "a" using command down
                keystroke replacementText
                delay 0.3
                keystroke "z" using command down
            end tell
        end tell

        delay 0.5

        set actualText to my readFrontDocumentContents()
        my closeFrontDocumentWithoutSaving()

        if actualText is originalText then
            return my formatResult("ScenarioE", "passed", startedAt, "Command+A and Command+Z behaved as expected in TextEdit.")
        end if

        return my formatResult("ScenarioE", "failed", startedAt, "Command-based shortcut parity check did not restore the original text.")
    on error errorMessage
        return my formatResult("ScenarioE", "failed", startedAt, errorMessage)
    end try
end runScenarioE


on resetTextEdit()
    tell application "TextEdit"
        activate
    end tell

    delay 0.5

    if my checkTextEditRunning() then
        try
            tell application "TextEdit"
                close every document saving no
            end tell
        end try
    end if
end resetTextEdit


on checkTextEditRunning()
    tell application "System Events"
        return exists process "TextEdit"
    end tell
end checkTextEditRunning


on createPlainDocument(documentContents)
    tell application "TextEdit"
        activate
        set newDocument to make new document with properties {text:documentContents}
    end tell

    delay 0.5
end createPlainDocument


on openDocument(filePath)
    tell application "TextEdit"
        activate
        open POSIX file filePath
    end tell

    delay 0.8
end openDocument


on saveFrontDocument(filePath)
    tell application "TextEdit"
        save front document in POSIX file filePath
    end tell

    delay 0.8
end saveFrontDocument


on closeFrontDocumentWithoutSaving()
    tell application "TextEdit"
        if (count of documents) is greater than 0 then
            close front document saving no
        end if
    end tell

    delay 0.4
end closeFrontDocumentWithoutSaving


on requestCloseFrontDocument()
    tell application "TextEdit"
        close front document saving ask
    end tell
end requestCloseFrontDocument


on setFrontDocumentContents(documentContents)
    tell application "TextEdit"
        set text of front document to documentContents
    end tell

    delay 0.4
end setFrontDocumentContents


on readFrontDocumentContents()
    tell application "TextEdit"
        return (text of front document) as string
    end tell
end readFrontDocumentContents


on buildLoremIpsumDocument(minimumWords)
    set seedSentence to "lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua"
    set wordCount to count words of seedSentence
    set repetitions to (minimumWords div wordCount) + 1
    set outputText to ""

    repeat repetitions times
        set outputText to outputText & seedSentence & " "
    end repeat

    if (length of outputText) > 0 then
        set outputText to (characters 1 thru -2 of outputText) as text
    end if

    return outputText
end buildLoremIpsumDocument


on ensureDirectory(directoryPath)
    do shell script "mkdir -p " & quoted form of directoryPath
end ensureDirectory


on formatResult(identifier, statusValue, startedAt, detailsValue)
    set durationMs to ((current date) - startedAt) * 1000
    return identifier & "|||" & statusValue & "|||" & durationMs & "|||" & detailsValue
end formatResult


on joinLines(lineItems)
    set previousDelimiters to AppleScript's text item delimiters

    set AppleScript's text item delimiters to linefeed
    set joinedText to lineItems as text

    set AppleScript's text item delimiters to previousDelimiters

    return joinedText
end joinLines