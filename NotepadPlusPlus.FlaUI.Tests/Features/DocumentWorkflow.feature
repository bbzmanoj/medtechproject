Feature: Document workflow

  Scenario: File round trip preserves large text
    Given Notepad++ is open
    And I have prepared large document content
    When I replace the editor content with the prepared text
    And I save the document as "round-trip.txt"
    And I reopen the saved file
    Then the editor should display the prepared text

  Scenario: Open dialog loads an existing file
    Given Notepad++ is open
    And an existing file contains "Pre-existing file content for open dialog validation."
    When I open the prepared file through the Open dialog
    Then the editor should display the prepared text

  Scenario: Unicode and encoding preserves utf-8 characters
    Given Notepad++ is open
    And I have prepared unicode document content
    When I replace the editor content with the prepared text
    And I save the document as UTF-8 file "unicode-round-trip.txt"
    And I reopen the saved file
    Then the editor should display the prepared text