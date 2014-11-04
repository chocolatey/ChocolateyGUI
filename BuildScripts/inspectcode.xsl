<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:key name="ISSUETYPES" match="/Report/Issues/Project/Issue" use="@TypeId"/>
  <xsl:output method="html" indent="yes"/>

  <xsl:template match="/" name="TopLevelReport">
    <html>
      <body>
        <head>
          <style>
            th {
            background-color: #f1f1c1;
            }
            table, th, td {
            border: 1px solid black;
            border-collapse: collapse;
            }
            th, td {
            padding: 5px;
            text-align: left;
            }
          </style>
        </head>
        <h1>Code Inspection Error Report</h1>

        <xsl:for-each select="/Report/IssueTypes/IssueType[@Severity='ERROR']">
          <table style="width:100%">
            <caption>
              <xsl:value-of select="@Description"/>
            </caption>
            <tr>
              <th>File</th>
              <th>Line Number</th>
              <th>Message</th>
            </tr>
            <xsl:for-each select="key('ISSUETYPES',@Id)">
              <tr>
                <td>
                  <xsl:value-of select="@File"/>
                </td>
                <td>
                  <xsl:value-of select="@Line"/>
                </td>
                <td>
                  <xsl:value-of select="@Message"/>
                </td>
              </tr>
            </xsl:for-each>
          </table>
          <br />
          <hr />
          <br />
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>