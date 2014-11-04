<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">	
	<xsl:output method="html"/>
	<xsl:variable name="stylecop.root" select="//StyleCopViolations"/>

	<xsl:template match="/">
		<div name="StyleCopViolations">
		
		<script language="javascript">
		<![CDATA[
			function expcol (sender, args){

				var el = document.getElementById (args);
				if (el.style.display == 'block') {
					el.style.display = 'none';
				} else {
					el.style.display = 'block';
				}
				
				tootleText(sender);
				for(var index = 2; index < arguments.length; index++){
					var elem = document.getElementById (arguments[index]);
					tootleText(elem);
				}
				
			}
			
			function tootleText(elem){
				if ('[show]' == elem.textContent){
					elem.textContent = '[hide]';
				}else if ('[hide]' == elem.textContent){
					elem.textContent = '[show]';
				}
			}
		]]>
		</script>
		
		<style type="text/css">
			li { line-height: 1.5em; padding-bottom: 5px;}
			*[name=missingInfo] { font-weight: bold;}
			div[name=StyleCopViolations] .sectionheader { width: 98%; padding: 2px;}
			.toc { background-color: #F6F6F6;
				border: 1px solid #DDDDDD;
				padding: 10px;
				margin-right: 10px;
				float: right;
				width: 400px;
				overflow: hidden;
				}
			div[name=StyleCopViolations] h2 { font-size: 1.2em;}
			
			div[name=StyleCopViolations] a { text-decoration: none;}
			div[name=StyleCopViolations] a:hover { text-decoration: underline; color: red;}
			.ruleBlock { background-color: #f6f6f6; border: solid 1px #dddddd;}
			.fileInfo {font-weight: bold;}
			.description { color: #60376F;}
		</style>
		
		<div class="sectionheader">StyleCop Report</div>
		<xsl:choose>
			<xsl:when test="count($stylecop.root) = 0">
				<span name="missingInfo">No StyleCop information in build logs.</span>
			</xsl:when>
		</xsl:choose>
		<xsl:apply-templates select="$stylecop.root"/>
		</div>
	</xsl:template>
	
	<xsl:template match="StyleCopViolations">
	
			<div name="header">
			
				<h1>StyleCop Report</h1>
				
				<span name="label">
					<xsl:choose>
						<xsl:when test="count(./Violation) > 0">
							<xsl:attribute name="class">error</xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
							<xsl:attribute name="class">success</xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>
					<a name="summary"></a>
					There were <xsl:value-of select="count(./Violation)" /> violations found, breaking 
					<xsl:value-of select="count(Violation[not(@Rule=preceding::Violation/@Rule)])" /> rules in 
					<xsl:value-of select="count(Violation[not(@Source=preceding::Violation/@Source)])" /> files
				</span>
				
			<!-- TOC -->
			<div class="toc">
			<div align="center">
				<b style="font-size: 10pt;">Table of contents</b>
			</div>
			<p style="font-size: 10pt;">
				<a href="#summary">1.&#160;&#160;Summary</a>
				<br />
				&#160;&#160;<a href="#toogleListOfRules" onclick="expcol(this, 'listOfRules', 'toogleListOfRules');">1.1.&#160;&#160;Rule violations per file report</a>
				<br />
				<a href="#mainreport">2.&#160;&#160;Reported rule violations</a>
				<br />
			<xsl:for-each select="Violation/@Rule[not(.=preceding::Violation/@Rule)]">
				<xsl:sort select="." />
				<xsl:variable name="rule">
							<xsl:value-of select="." />
				</xsl:variable>
				&#160;&#160;<a href="#{$rule}">2.<xsl:value-of select="position()" />.&#160;<xsl:value-of select="$rule" /><br />
				</a>
			</xsl:for-each>
			</p>
			</div>
			
			<!-- List of violations per file -->
				<h2 id="filesDetails">Rule violations per file report
				<a href="#" id="toogleListOfRules" onclick="expcol(this, 'listOfRules'); return false;">[show]</a>
				</h2>
				<br />
				<div id="listOfRules" style="display:none;">
				<ul >
				<xsl:for-each select="Violation[not(@Source=preceding::Violation/@Source)]">
					<xsl:sort select="./@Source" />
					<xsl:variable name="filePath">
								<xsl:value-of select="./@Source" />
					</xsl:variable>
					<li><xsl:value-of select="@Source" /> : <xsl:value-of select="count(//Violation[@Source=$filePath])" /> violation(s)</li>
				</xsl:for-each>
				</ul>
				<a href="#filesDetails" onclick="expcol(this, 'listOfRules', 'toogleListOfRules');">[Hide details per file]</a>
				</div>
			</div>
			
			<!-- List of violations per rule -->
			<h2><a name="mainreport"></a>Reported rule violations</h2>
			<ol name="errorList">
				<xsl:for-each select="Violation[not(@Rule=preceding::Violation/@Rule)]">
					<xsl:sort select="@Rule" />
					<xsl:variable name="rule">
								<xsl:value-of select="./@Rule" />
					</xsl:variable>
					<h3><xsl:value-of select="position()" />.&#160;<xsl:value-of select="@Rule" />&#160;
					<a href="#" onclick="expcol(this, 'ruleDetails{$rule}'); return false;"><xsl:attribute name="name"><xsl:value-of select="./@Rule" /></xsl:attribute>[hide]</a>
					</h3>
					<div id="ruleDetails{$rule}" style="display:block;" > 
					
					<h4><xsl:value-of select="count(//Violation[@Rule=$rule])" /> violation(s) found</h4>
					
					<ol class="ruleBlock">
					<xsl:for-each select="//Violation[@Rule=$rule]">
						<xsl:sort select="@Source" />
						<xsl:sort select="@LineNumber" />
						<li>
							<span class="fileInfo"><xsl:value-of select="concat(@Source, ' : ', @LineNumber)" /></span><br/>
							<span class="description"><xsl:value-of select="." /></span>
						</li>
						</xsl:for-each>
					</ol>
					</div>
				</xsl:for-each>
			</ol>
    </xsl:template>
	
</xsl:stylesheet>
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               