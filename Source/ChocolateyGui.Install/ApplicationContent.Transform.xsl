<?xml version="1.0" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://schemas.microsoft.com/wix/2006/wi">

  <!-- Copy all attributes and elements to the output. -->
  <xsl:template match="@*|*">
    <xsl:copy>
      <xsl:apply-templates select="@*" />
      <xsl:apply-templates select="*" />
    </xsl:copy>
  </xsl:template>

  <xsl:output method="xml" indent="yes" />

  <xsl:key name="exe-search" match="wix:Component[contains(wix:File/@Source, 'ChocolateyGui.exe')]" use="@Id" />
  <xsl:template match="wix:Component[key('exe-search', @Id)]" />
  <xsl:template match="wix:ComponentRef[key('exe-search', @Id)]" />

  <xsl:key name="dll-search" match="wix:Component[contains(wix:File/@Source, 'ChocolateyGui.Common.dll')]" use="@Id" />
  <xsl:template match="wix:Component[key('dll-search', @Id)]" />
  <xsl:template match="wix:ComponentRef[key('dll-search', @Id)]" />

  <xsl:key name="dll-search" match="wix:Component[contains(wix:File/@Source, 'ChocolateyGui.Common.Windows.dll')]" use="@Id" />
  <xsl:template match="wix:Component[key('dll-search', @Id)]" />
  <xsl:template match="wix:ComponentRef[key('dll-search', @Id)]" />

  <xsl:key name="vshost-search" match="wix:Component[contains(wix:File/@Source, 'vshost')]" use="@Id" />
  <xsl:template match="wix:Component[key('vshost-search', @Id)]" />
  <xsl:template match="wix:ComponentRef[key('vshost-search', @Id)]" />

  <xsl:key name="vshost-search" match="wix:Component[contains(wix:File/@Source, '.xml')]" use="@Id" />
  <xsl:template match="wix:Component[key('vshost-search', @Id)]" />
  <xsl:template match="wix:ComponentRef[key('vshost-search', @Id)]" />

  <xsl:key name="d3d-search" match="wix:Component[contains(wix:File/@Source, 'd3dcompiler_43.dll')]" use="@Id" />
  <xsl:template match="wix:Component[key('d3d-search', @Id)]" />
  <xsl:template match="wix:ComponentRef[key('d3d-search', @Id)]" />
</xsl:stylesheet>