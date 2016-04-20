html常用标签：
	表单：form,label,input,button,select,textarea
	表格：table,tr,th,td,thead,tbody,tfoot,caption
	项目列表：ul,ol,li,dl,dt,dd
	文字：p,font,b,strong,span,br,i,center,u
	标题：h1,h2,h3,h4,h5,h6
	页头：title,meta,base
	其他：div,script,style,link,base,iframe,frame,frameset,hr,img,map
	不常用：applet,object,embed,code
css常用样式属性：
	字体：font,color,font-family,font-size,font-weight,font-style,text-decoration,text-shadow(不常用),line-height,letter-spacing,word-spacing
	文本：text-indent,text-overflow,vertical-align,text-align,word-break,line-break(我没有用过),word-wrap,white-space
	背景：background,background-color,background-image,background-position,background-positionX,background-positionY,background-repeat
	定位：position(IE6不支持fixed),z-index,top,right,bottom,left
	尺寸：height,max-height,min-height,width,max-width,min-width  === 代有max-和min-的IE6要注意
	布局：clear,float,overflow,overflow-x,overflow-y,display,visibility
	外补丁（外边距）：margin,margin-top,margin-right,margin-bottom,margin-left
	内补丁（内边距）：padding,padding-top,padding-right,padding-bottom,padding-left
	边框：border,border-color,border-style,border-width,border-top,border-top-color,border-top-style,border-top-width,border-right,border-right-color,border-right-style,border-right-width,border-bottom,border-bottom-color,border-bottom-style,border-bottom-width,border-left,border-left-color,border-left-style,border-left-width
	内容：content
	列表：list-style,list-style-image,list-style-position,list-style-type
	表格：border-collapse
	滚动条：很不常用可以看一下
	其它：cursor,behavior
	选择符：把所设置样式应用与哪些元素，非常重要
	规则：@import,@charset
	声明（强制使用/应用/解析）：!important
	单位：px,em,#RRGGBB,rgb(R,G,B),Color Name
	滤镜（filters）：只有IE有效

----------------------------------------------------------------------------------------
----------------------------------下列内容纯属经验之谈----------------------------------
----------------------------------------------------------------------------------------

float注意事项：
	1.有事IE解析float:left时，margin和padding值变量不是设置的实际大小，可以加上display:inline解决，还是不能解决只好对不同浏览器设置
	2.有时间在float时，最好限一下宽或高，防止在IE下不正常
position注意事项：
	1.position:absolute是相对于父的position:relative，如果没有父级为position:relative是相对与页面的
	2.别忘了每个元素的默认position值是static
标签性质（只列了我常用的）：
	块级标签：form,div,iframe,frame,frameset,hr,p,ul,ol,li,dl,dt,dd,center == 其实表格相关也算是
	内联标签：label,input,button,select,textarea,font,b,strong,span,br,i,u
	注意事项：
		1.以上俩种可以用CSS(display样式属性)来改变其性质
		2.设置尺寸样式时要注意，只有块级标签有效的,或者用样式设置内联标签为display:block也是可以的
标签嵌套注意事项：
	1.不能嵌套h1,h2,h3,h4,h5,h6的标签：p,dt,caption
	1.h1,h2,h3,h4,h5,h6标签不能嵌套：p,div等块级标签
