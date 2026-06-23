import{a as Se}from"./chunk-WNP4CGEX.js";import{a as b,b as H}from"./chunk-JJHXS3WI.js";import"./chunk-RVO6OU6F.js";import"./chunk-EQWWJNDK.js";import"./chunk-QU4D4DC5.js";import"./chunk-FCI6BDFT.js";import"./chunk-DN3SMWXA.js";import"./chunk-M37S6YIG.js";import{a as ze}from"./chunk-Y5AE6XZY.js";import{a as Ce}from"./chunk-NEN7Y75B.js";import"./chunk-MRIAAZ4U.js";import{ca as be,da as ve,ga as ye,ia as _e,ja as he,ka as A}from"./chunk-YSBBJ5GQ.js";import"./chunk-5FO3BX5S.js";import{d as fe}from"./chunk-VPAYVVCU.js";import{a as R}from"./chunk-W7QCB7FD.js";import"./chunk-EWDIBSXK.js";import{g as V,i as ue,k as ce,n as ge}from"./chunk-YB3FJLY5.js";import{$a as q,Bb as se,Db as J,Eb as K,Ib as E,Jb as I,Lb as w,Mb as s,Nb as S,Oa as m,Ob as le,P as X,Pb as h,S as Z,Sa as te,Ta as ne,U as p,Ua as W,Wb as D,Yb as B,Z as y,_,cb as P,db as Q,eb as ie,gb as re,hb as oe,ib as d,jb as a,kb as l,kc as N,lb as u,mb as g,na as Y,nb as f,ob as T,pc as pe,qc as F,rb as z,sb as M,wb as v,xb as ae,xc as de,ya as ee,yb as c,yc as me,za as o}from"./chunk-D5MI7QOI.js";function we(t,i){t&1&&(a(0,"span"),s(1,"Unflag"),l())}function Ae(t,i){t&1&&(a(0,"span"),s(1,"Flag"),l())}var j=class t{mapperService=p(H);quizAttemptStore=p(b);questionType=F.required();onClickFlag(){this.quizAttemptStore.changeFlagStatusForTheCurrentQuestion()}static \u0275fac=function(e){return new(e||t)};static \u0275cmp=m({type:t,selectors:[["app-question-attempt-header"]],inputs:{questionType:[1,"questionType"]},decls:6,vars:4,consts:[[1,"question-attempt-header"],[3,"ngComponentOutlet"],["type","button","aria-label","Flag question",1,"flag","btn",3,"click"],["aria-hidden","true",1,"fa-solid","fa-circle-exclamation"]],template:function(e,n){e&1&&(a(0,"header",0),z(1,1),a(2,"button",2),v("click",function(){return n.onClickFlag()}),u(3,"i",3),P(4,we,2,0,"span")(5,Ae,2,0,"span"),l()()),e&2&&(o(),d("ngComponentOutlet",n.mapperService.getSuitableQuestionTag(n.questionType())),o(),I("flagged",n.quizAttemptStore.isCurrentQuestionFlagged()),o(2),Q(n.quizAttemptStore.isCurrentQuestionFlagged()?4:5))},dependencies:[V],styles:[".question-attempt-header[_ngcontent-%COMP%]{display:flex;align-items:center;justify-content:space-between;gap:.75rem}.flag[_ngcontent-%COMP%]{display:inline-flex;align-items:center;gap:.5rem;padding:.4rem .85rem;border:1px solid var(--clr-gray-300);border-radius:var(--radius-lg);background-color:var(--clr-gray-100);color:var(--clr-gray-600);font-size:var(--fs-600);font-weight:500;line-height:1}.flagged[_ngcontent-%COMP%]{border-color:var(--clr-yellow-500);background-color:var(--clr-yellow-500);color:var(--clr-red-500)}"],changeDetection:0})};function ke(t,i){if(t&1){let e=M();g(0,"button",7),ae("click",function(){let r=y(e).$index,C=c();return _(C.onClick(r))}),s(1),f()}if(t&2){let e=i.$implicit,n=i.$index,r=c();I("is-current",n===r.quizAttemptStore.currentQuestionIndex())("is-flagged",e.isFlagged)("is-solved",e.isSolved),o(),le(" ",n+1," ")}}var G=class t{quizAttemptStore=p(b);onClick(i){this.quizAttemptStore.setCurrentQuestionIndex(i)}static \u0275fac=function(e){return new(e||t)};static \u0275cmp=m({type:t,selectors:[["app-questions-navigator"]],decls:16,vars:0,consts:[["aria-label","Question navigator",1,"navigator-card"],[1,"navigator-grid"],["type","button",3,"is-current","is-flagged","is-solved"],["aria-label","Question status legend",1,"legend"],[1,"dot","answered"],[1,"dot","unanswered"],[1,"dot","flagged"],["type","button",3,"click"]],template:function(e,n){e&1&&(g(0,"section",0)(1,"h2"),s(2,"Question Navigator"),f(),g(3,"div",1),re(4,ke,2,7,"button",2,ie),f(),g(6,"ul",3)(7,"li"),T(8,"span",4),s(9,"Answered"),f(),g(10,"li"),T(11,"span",5),s(12,"Unanswered"),f(),g(13,"li"),T(14,"span",6),s(15,"Flagged"),f()()()),e&2&&(o(4),oe(n.quizAttemptStore.quizQuestions()))},styles:["[_nghost-%COMP%]{display:block}.navigator-card[_ngcontent-%COMP%]{display:grid;gap:.75rem;padding:1rem;border:1px solid var(--clr-gray-300);border-radius:.75rem;background:var(--clr-white)}h2[_ngcontent-%COMP%]{margin:0;font-size:1rem}.navigator-grid[_ngcontent-%COMP%]{display:grid;grid-template-columns:repeat(5,minmax(2rem,1fr));gap:.5rem}button[_ngcontent-%COMP%]{border:1px solid var(--clr-gray-300);border-radius:.625rem;min-height:2rem;background:var(--clr-gray-100);font-weight:600;color:var(--clr-gray-500)}button.is-solved[_ngcontent-%COMP%]{background:var(--clr-green-500);color:var(--clr-white);border-color:var(--clr-green-500)}button.is-flagged[_ngcontent-%COMP%]{background:var(--clr-yellow-500);color:var(--clr-red-500);border-color:var(--clr-yellow-500)}button.is-current[_ngcontent-%COMP%]{background:var(--clr-white);color:var(--clr-green-500);border:2px solid var(--clr-green-500)}.legend[_ngcontent-%COMP%]{list-style:none;padding:0;margin:0;display:grid;gap:.25rem;font-size:.875rem;color:var(--clr-gray-600)}.legend[_ngcontent-%COMP%]   li[_ngcontent-%COMP%]{display:flex;align-items:center;gap:.5rem}.dot[_ngcontent-%COMP%]{width:.625rem;height:.625rem;border-radius:999px;display:inline-block}.answered[_ngcontent-%COMP%]{background:var(--clr-green-500)}.unanswered[_ngcontent-%COMP%]{background:var(--clr-gray-300)}.flagged[_ngcontent-%COMP%]{background:var(--clr-yellow-500)}"],changeDetection:0})};var xe=`
    .p-progressbar {
        display: block;
        position: relative;
        overflow: hidden;
        height: dt('progressbar.height');
        background: dt('progressbar.background');
        border-radius: dt('progressbar.border.radius');
    }

    .p-progressbar-value {
        margin: 0;
        background: dt('progressbar.value.background');
    }

    .p-progressbar-label {
        color: dt('progressbar.label.color');
        font-size: dt('progressbar.label.font.size');
        font-weight: dt('progressbar.label.font.weight');
    }

    .p-progressbar-determinate .p-progressbar-value {
        height: 100%;
        width: 0%;
        position: absolute;
        display: none;
        display: flex;
        align-items: center;
        justify-content: center;
        overflow: hidden;
        transition: width 1s ease-in-out;
    }

    .p-progressbar-determinate .p-progressbar-label {
        display: inline-flex;
    }

    .p-progressbar-indeterminate .p-progressbar-value::before {
        content: '';
        position: absolute;
        background: inherit;
        inset-block-start: 0;
        inset-inline-start: 0;
        inset-block-end: 0;
        will-change: inset-inline-start, inset-inline-end;
        animation: p-progressbar-indeterminate-anim 2.1s cubic-bezier(0.65, 0.815, 0.735, 0.395) infinite;
    }

    .p-progressbar-indeterminate .p-progressbar-value::after {
        content: '';
        position: absolute;
        background: inherit;
        inset-block-start: 0;
        inset-inline-start: 0;
        inset-block-end: 0;
        will-change: inset-inline-start, inset-inline-end;
        animation: p-progressbar-indeterminate-anim-short 2.1s cubic-bezier(0.165, 0.84, 0.44, 1) infinite;
        animation-delay: 1.15s;
    }

    @keyframes p-progressbar-indeterminate-anim {
        0% {
            inset-inline-start: -35%;
            inset-inline-end: 100%;
        }
        60% {
            inset-inline-start: 100%;
            inset-inline-end: -90%;
        }
        100% {
            inset-inline-start: 100%;
            inset-inline-end: -90%;
        }
    }
    @-webkit-keyframes p-progressbar-indeterminate-anim {
        0% {
            inset-inline-start: -35%;
            inset-inline-end: 100%;
        }
        60% {
            inset-inline-start: 100%;
            inset-inline-end: -90%;
        }
        100% {
            inset-inline-start: 100%;
            inset-inline-end: -90%;
        }
    }

    @keyframes p-progressbar-indeterminate-anim-short {
        0% {
            inset-inline-start: -200%;
            inset-inline-end: 100%;
        }
        60% {
            inset-inline-start: 107%;
            inset-inline-end: -8%;
        }
        100% {
            inset-inline-start: 107%;
            inset-inline-end: -8%;
        }
    }
    @-webkit-keyframes p-progressbar-indeterminate-anim-short {
        0% {
            inset-inline-start: -200%;
            inset-inline-end: 100%;
        }
        60% {
            inset-inline-start: 107%;
            inset-inline-end: -8%;
        }
        100% {
            inset-inline-start: 107%;
            inset-inline-end: -8%;
        }
    }
`;var Oe=["content"],Te=t=>({$implicit:t});function Ee(t,i){if(t&1&&(a(0,"div"),s(1),l()),t&2){let e=c(2);E("display",e.value!=null&&e.value!==0?"flex":"none"),o(),h("",e.value,"",e.unit)}}function Ie(t,i){t&1&&z(0)}function De(t,i){if(t&1&&(a(0,"div",2)(1,"div",2),W(2,Ee,2,4,"div",3)(3,Ie,1,0,"ng-container",4),l()()),t&2){let e=c();w(e.cn(e.cx("value"),e.valueStyleClass)),E("width",e.value+"%")("display","flex")("background",e.color),d("pBind",e.ptm("value")),q("data-p",e.dataP),o(),w(e.cx("label")),d("pBind",e.ptm("label")),q("data-p",e.dataP),o(),d("ngIf",e.showValue&&!e.contentTemplate&&!e._contentTemplate),o(),d("ngTemplateOutlet",e.contentTemplate||e._contentTemplate)("ngTemplateOutletContext",B(17,Te,e.value))}}function Be(t,i){if(t&1&&u(0,"div",2),t&2){let e=c();w(e.cn(e.cx("value"),e.valueStyleClass)),E("background",e.color),d("pBind",e.ptm("value")),q("data-p",e.dataP)}}var Ne={root:({instance:t})=>["p-progressbar p-component",{"p-progressbar-determinate":t.mode=="determinate","p-progressbar-indeterminate":t.mode=="indeterminate"}],value:"p-progressbar-value",label:"p-progressbar-label"},qe=(()=>{class t extends ye{name="progressbar";style=xe;classes=Ne;static \u0275fac=(()=>{let e;return function(r){return(e||(e=Y(t)))(r||t)}})();static \u0275prov=X({token:t,factory:t.\u0275fac})}return t})();var Pe=new Z("PROGRESSBAR_INSTANCE"),Qe=(()=>{class t extends he{componentName="ProgressBar";$pcProgressBar=p(Pe,{optional:!0,skipSelf:!0})??void 0;bindDirectiveInstance=p(A,{self:!0});value;showValue=!0;styleClass;valueStyleClass;unit="%";mode="determinate";color;contentTemplate;onAfterViewChecked(){this.bindDirectiveInstance.setAttrs(this.ptms(["host","root"]))}_componentStyle=p(qe);templates;_contentTemplate;onAfterContentInit(){this.templates?.forEach(e=>{e.getType()==="content"?this._contentTemplate=e.template:this._contentTemplate=e.template})}get dataP(){return this.cn({determinate:this.mode==="determinate",indeterminate:this.mode==="indeterminate"})}static \u0275fac=(()=>{let e;return function(r){return(e||(e=Y(t)))(r||t)}})();static \u0275cmp=m({type:t,selectors:[["p-progressBar"],["p-progressbar"],["p-progress-bar"]],contentQueries:function(n,r,C){if(n&1&&se(C,Oe,4)(C,be,4),n&2){let O;J(O=K())&&(r.contentTemplate=O.first),J(O=K())&&(r.templates=O)}},hostAttrs:["role","progressbar"],hostVars:7,hostBindings:function(n,r){n&2&&(q("aria-valuemin",0)("aria-valuenow",r.value)("aria-valuemax",100)("aria-level",r.value+r.unit)("data-p",r.dataP),w(r.cn(r.cx("root"),r.styleClass)))},inputs:{value:[2,"value","value",me],showValue:[2,"showValue","showValue",de],styleClass:"styleClass",valueStyleClass:"valueStyleClass",unit:"unit",mode:"mode",color:"color"},features:[D([qe,{provide:Pe,useExisting:t},{provide:_e,useExisting:t}]),te([A]),ne],decls:2,vars:2,consts:[[3,"class","pBind","width","display","background",4,"ngIf"],[3,"class","pBind","background",4,"ngIf"],[3,"pBind"],[3,"display",4,"ngIf"],[4,"ngTemplateOutlet","ngTemplateOutletContext"]],template:function(n,r){n&1&&W(0,De,4,19,"div",0)(1,Be,1,6,"div",1),n&2&&(d("ngIf",r.mode==="determinate"),o(),d("ngIf",r.mode==="indeterminate"))},dependencies:[ge,ue,ce,ve,A],encapsulation:2,changeDetection:0})}return t})();var $=class t{quizAttemptStore=p(b);progressValue=N(()=>{let i=this.quizAttemptStore.numberOfQuestions();return i===0?0:Math.round(this.quizAttemptStore.numberOfSolvedQuestions()/i*100)});static \u0275fac=function(e){return new(e||t)};static \u0275cmp=m({type:t,selectors:[["app-questions-progress-bar"]],decls:4,vars:4,consts:[["aria-label","Quiz progress",1,"progress-card"],["aria-label","Solved questions progress",1,"quiz-progress",3,"value","showValue"],[1,"progress-summary"]],template:function(e,n){e&1&&(a(0,"section",0),u(1,"p-progressbar",1),a(2,"p",2),s(3),l()()),e&2&&(o(),d("value",n.progressValue())("showValue",!1),o(2),h(" ",n.quizAttemptStore.numberOfSolvedQuestions()," of ",n.quizAttemptStore.numberOfQuestions()," answered "))},dependencies:[Qe],styles:["[_nghost-%COMP%]{display:block}.progress-card[_ngcontent-%COMP%]{display:grid;gap:.5rem;padding:1rem;border:1px solid var(--clr-gray-300);border-radius:.75rem;background:var(--clr-white)}.quiz-progress[_ngcontent-%COMP%]{height:1rem;border-radius:var(--radius-lg);background:var(--clr-gray-200)}.quiz-progress[_ngcontent-%COMP%]   .p-progressbar-value[_ngcontent-%COMP%]{border-radius:var(--radius-lg);background:var(--gradient-main)}.progress-summary[_ngcontent-%COMP%]{margin:0;text-align:center;font-size:var(--fs-300);color:var(--clr-gray-600);font-weight:600}"],changeDetection:0})};var L=class t{quizAttemptStore=p(b);remainingTime=N(()=>{let i=this.quizAttemptStore.remaningSeconds(),e=Math.floor(i/60),n=i%60;return`${e.toString().padStart(2,"0")}:${n.toString().padStart(2,"0")}`});static \u0275fac=function(e){return new(e||t)};static \u0275cmp=m({type:t,selectors:[["app-quiz-attempt-header"]],decls:11,vars:6,consts:[[1,"attempt-header"],["aria-label","Quiz status",1,"attempt-meta"],[1,"chip"]],template:function(e,n){e&1&&(g(0,"header",0)(1,"div")(2,"h1"),s(3),f(),g(4,"p"),s(5),f()(),g(6,"div",1)(7,"span",2),s(8),f(),g(9,"span",2),s(10),f()()()),e&2&&(o(3),S(n.quizAttemptStore.quizTitle()),o(2),h(" Question ",n.quizAttemptStore.currentQuestionIndex()," of ",n.quizAttemptStore.numberOfQuestions()," "),o(3),h("",n.quizAttemptStore.numberOfSolvedQuestions(),"/",n.quizAttemptStore.numberOfQuestions()),o(2),S(n.remainingTime()))},styles:["[_nghost-%COMP%]{display:block}.attempt-header[_ngcontent-%COMP%]{display:flex;justify-content:space-between;align-items:center;gap:.75rem;padding:1rem;border:1px solid var(--clr-gray-300);border-radius:.75rem;background:var(--clr-white)}h1[_ngcontent-%COMP%]{margin:0;font-size:1.25rem}p[_ngcontent-%COMP%]{margin:.25rem 0 0;color:var(--clr-gray-600);font-size:.875rem}.attempt-meta[_ngcontent-%COMP%]{display:flex;gap:.5rem;flex-wrap:wrap;justify-content:end}.chip[_ngcontent-%COMP%]{padding:.35rem .65rem;border:1px solid var(--clr-gray-300);border-radius:999px;font-size:.875rem;font-weight:600;color:var(--clr-gray-600)}@media(width<=40rem){.attempt-header[_ngcontent-%COMP%]{flex-direction:column;align-items:flex-start}}"],changeDetection:0})};var U=class t{seeResults=pe();static \u0275fac=function(e){return new(e||t)};static \u0275cmp=m({type:t,selectors:[["app-quiz-finished-message"]],outputs:{seeResults:"seeResults"},decls:9,vars:0,consts:[[1,"quiz-completed-card"],[1,"quiz-completed-card__icon"],[1,"fa-solid","fa-circle-check"],[1,"quiz-completed-card__title"],[1,"quiz-completed-card__message"],["appButton","","variant","green","type","button",1,"quiz-completed-card__btn",3,"click"]],template:function(e,n){e&1&&(a(0,"div",0)(1,"div",1),u(2,"i",2),l(),a(3,"h2",3),s(4,"Quiz Completed!"),l(),a(5,"p",4),s(6," You Have Compelete the Quiz Take a Rest Before you See you results \u{1F609} "),l(),a(7,"button",5),v("click",function(){return n.seeResults.emit()}),s(8," See Results "),l()())},dependencies:[R],styles:["[_nghost-%COMP%]{display:block;width:100%}.quiz-completed-card[_ngcontent-%COMP%]{display:grid;place-items:center;align-content:center;gap:1.5rem;padding:3.5rem 2rem;background-color:var(--clr-white);border:1px solid var(--clr-gray-200);border-radius:var(--radius-lg);box-shadow:0 10px 15px -3px #0000000d,0 4px 6px -2px #00000005;max-width:32rem;width:100%;margin:4rem auto;text-align:center;animation:_ngcontent-%COMP%_fadeInUp .4s cubic-bezier(.16,1,.3,1)}.quiz-completed-card__icon[_ngcontent-%COMP%]{font-size:3.5rem;color:var(--clr-green-500);animation:_ngcontent-%COMP%_scaleIn .5s cubic-bezier(.34,1.56,.64,1)}.quiz-completed-card__title[_ngcontent-%COMP%]{font-size:1.8rem;font-weight:700;color:var(--clr-blue-900);margin:0}.quiz-completed-card__message[_ngcontent-%COMP%]{font-size:1.1rem;line-height:1.6;color:var(--clr-gray-600);margin:0}.quiz-completed-card__btn[_ngcontent-%COMP%]{min-width:12rem}@keyframes _ngcontent-%COMP%_fadeInUp{0%{opacity:0;transform:translateY(20px)}to{opacity:1;transform:translateY(0)}}@keyframes _ngcontent-%COMP%_scaleIn{0%{opacity:0;transform:scale(.5)}to{opacity:1;transform:scale(1)}}"],changeDetection:0})};var Fe=t=>({question:t});function Ve(t,i){t&1&&(a(0,"div",1),u(1,"p-progress-spinner",2),l())}function Re(t,i){t&1&&(a(0,"app-operation-failed")(1,"p"),s(2),l()()),t&2&&(o(2),S(i))}function He(t,i){if(t&1){let e=M();a(0,"app-quiz-finished-message",3),v("seeResults",function(){y(e);let r=c();return _(r.goToResults())}),l()}}function je(t,i){t&1&&(a(0,"app-operation-failed")(1,"p"),s(2),l()()),t&2&&(o(2),S(i))}function Ge(t,i){if(t&1){let e=M();u(0,"app-quiz-attempt-header"),P(1,je,3,1,"app-operation-failed"),a(2,"div",4)(3,"div",5),u(4,"app-question-attempt-header",6),z(5,7),a(6,"app-navigation-buttons",8),v("previousButtonClicked",function(){y(e);let r=c();return _(r.quizAttemptStore.GoToPreviousQuestion())})("nextButtonClicked",function(){y(e);let r=c();return _(r.quizAttemptStore.GoToNextQuestion())}),l()(),a(7,"aside",9),u(8,"app-questions-navigator")(9,"app-questions-progress-bar"),a(10,"button",10),v("click",function(){y(e);let r=c();return _(r.quizAttemptStore.SubmitQuiz())}),s(11," Submit Quiz "),l()()()}if(t&2){let e,n=c();o(),Q((e=n.quizAttemptStore.error()("submit"))?1:-1,e);let r=n.quizAttemptStore.quizQuestions()[n.quizAttemptStore.currentQuestionIndex()];o(3),d("questionType",r.type),o(),d("ngComponentOutlet",n.questionMapperService.getSuitableQuestionAttemptComponent(r.type))("ngComponentOutletInputs",B(7,Fe,r)),o(),d("canGoPrevious",n.quizAttemptStore.canGoPrevious())("canGoNext",n.quizAttemptStore.canGoNext()),o(4),d("loading",n.quizAttemptStore.isPending()("submit"))}}var Me=class t{questionMapperService=p(H);quizId=F.required();quizAttemptStore=p(b);router=p(fe);ngOnInit(){this.quizAttemptStore.load({quizId:this.quizId()})}unloadNotification(i){this.isQuizInProgress()&&i.preventDefault()}canDeactivate(){return this.isQuizInProgress()?confirm("Are you sure you want to leave? Your progress will be lost and the quiz will not be submitted."):!0}goToResults(){this.router.navigate(["/student/results"])}isQuizInProgress(){let i=this.quizAttemptStore,e=!i.isPending()("load")&&!i.error()("load"),n=i.isFulfilled()("submit");return e&&!n}static \u0275fac=function(e){return new(e||t)};static \u0275cmp=m({type:t,selectors:[["app-quiz-attempt"]],hostBindings:function(e,n){e&1&&v("beforeunload",function(C){return n.unloadNotification(C)},ee)},inputs:{quizId:[1,"quizId"]},features:[D([b])],decls:5,vars:1,consts:[["aria-label","Quiz attempt layout",1,"attempt-layout"],[1,"spinner-container"],["ariaLabel","Loading quiz attempt"],[3,"seeResults"],[1,"attempt-main"],["aria-label","Question area",1,"question-column"],[3,"questionType"],[3,"ngComponentOutlet","ngComponentOutletInputs"],["ariaLabel","Question navigation",3,"previousButtonClicked","nextButtonClicked","canGoPrevious","canGoNext"],["aria-label","Quiz tools",1,"sidebar-column"],["appButton","","variant","red","type","button",2,"width","100%",3,"click","loading"]],template:function(e,n){if(e&1&&(a(0,"section",0),P(1,Ve,2,0,"div",1)(2,Re,3,1,"app-operation-failed")(3,He,1,0,"app-quiz-finished-message")(4,Ge,12,9),l()),e&2){let r;o(),Q(n.quizAttemptStore.isPending()("load")?1:(r=n.quizAttemptStore.error()("load"))?2:n.quizAttemptStore.isFulfilled()("submit")?3:4,r)}},dependencies:[L,G,Se,j,V,$,Ce,ze,R,U],styles:["[_nghost-%COMP%]{display:block;padding:1rem;width:100%}.attempt-layout[_ngcontent-%COMP%]{display:grid;gap:1rem;width:min(100%,70rem);margin:0 auto}.attempt-main[_ngcontent-%COMP%]{display:grid;gap:1rem;grid-template-columns:2fr 1fr;align-items:start}.question-column[_ngcontent-%COMP%], .sidebar-column[_ngcontent-%COMP%]{display:grid;gap:1rem}.spinner-container[_ngcontent-%COMP%]{display:flex;align-items:center;justify-content:center;min-height:20rem}@media(width<=64rem){.attempt-main[_ngcontent-%COMP%]{grid-template-columns:1fr}}"],changeDetection:0})};export{Me as QuizAttempt};
