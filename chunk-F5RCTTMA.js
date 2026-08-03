import{a as Me}from"./chunk-JPLKCSSW.js";import"./chunk-63YWVACR.js";import"./chunk-AFUUH34V.js";import{a as Qe}from"./chunk-7IW5B4BC.js";import{c as C,d as R}from"./chunk-G2Y7RFQ2.js";import"./chunk-LHU3HSFG.js";import"./chunk-OEY6TLYC.js";import"./chunk-EZUW2TZA.js";import"./chunk-OVC4TIUB.js";import"./chunk-D7UKVFWL.js";import"./chunk-IFVYPJ5L.js";import"./chunk-HJCMNBWB.js";import"./chunk-VAL746P7.js";import{a as qe}from"./chunk-JVFZRR2C.js";import{a as we}from"./chunk-NFIMHQOT.js";import"./chunk-CTNVO6IM.js";import{k as L}from"./chunk-RULHMY53.js";import{b as be,d as _e}from"./chunk-7EVCNOVE.js";import{c as xe}from"./chunk-77QDAAEP.js";import"./chunk-VOOEJBGF.js";import{da as ye,ea as Ce,ia as he,ka as ze,la as Se,ma as k}from"./chunk-5I7OODZZ.js";import{Ab as K,Bb as X,Cc as V,Ec as ge,Fb as I,Gb as D,Gc as fe,Ib as T,Jb as l,Jc as ve,Ka as c,Kb as x,Lb as pe,M as ee,Mb as q,Na as ie,Oa as re,P as te,Pa as J,R as p,Tb as B,Vb as N,W as f,X as v,_a as P,bb as h,cb as z,db as oe,fb as ae,gb as se,gc as Q,ha as U,hb as m,ib as a,jb as s,kb as g,la as W,lb as b,lc as me,mb as _,mc as F,n as Z,nb as E,qb as M,rb as w,rc as ue,sc as ce,tb as y,ub as le,vb as u,wa as ne,xa as o,yb as de}from"./chunk-P4O63RKO.js";import"./chunk-HIUAR4UN.js";function Ee(t,i){t&1&&(a(0,"span"),l(1,"Unflag"),s())}function Ie(t,i){t&1&&(a(0,"span"),l(1,"Flag"),s())}var H=class t{mapperService=p(R);quizAttemptStore=p(C);questionType=F.required();onClickFlag(){this.quizAttemptStore.changeFlagStatusForTheCurrentQuestion()}static \u0275fac=function(e){return new(e||t)};static \u0275cmp=c({type:t,selectors:[["app-question-attempt-header"]],inputs:{questionType:[1,"questionType"]},decls:6,vars:4,consts:[[1,"question-attempt-header"],[3,"ngComponentOutlet"],["type","button","aria-label","Flag question",1,"flag","btn",3,"click"],["aria-hidden","true",1,"fa-solid","fa-circle-exclamation"]],template:function(e,n){e&1&&(a(0,"header",0),M(1,1),a(2,"button",2),y("click",function(){return n.onClickFlag()}),g(3,"i",3),h(4,Ee,2,0,"span")(5,Ie,2,0,"span"),s()()),e&2&&(o(),m("ngComponentOutlet",n.mapperService.getSuitableQuestionTag(n.questionType())),o(),D("flagged",n.quizAttemptStore.isCurrentQuestionFlagged()),o(2),z(n.quizAttemptStore.isCurrentQuestionFlagged()?4:5))},dependencies:[V],styles:[".question-attempt-header[_ngcontent-%COMP%]{display:flex;align-items:center;justify-content:space-between;gap:.75rem}.flag[_ngcontent-%COMP%]{display:inline-flex;align-items:center;gap:.5rem;padding:.4rem .85rem;border:1px solid var(--clr-gray-300);border-radius:var(--radius-lg);background-color:var(--clr-gray-100);color:var(--clr-gray-600);font-size:var(--fs-600);font-weight:500;line-height:1}.flagged[_ngcontent-%COMP%]{border-color:var(--clr-amber-100);background-color:var(--clr-amber-100);color:var(--clr-red-500)}"],changeDetection:0})};function De(t,i){if(t&1){let e=w();b(0,"button",7),le("click",function(){let r=f(e).$index,d=u();return v(d.onClick(r))}),l(1),_()}if(t&2){let e=i.$implicit,n=i.$index,r=u();D("is-current",n===r.quizAttemptStore.currentQuestionIndex())("is-flagged",e.isFlagged)("is-solved",e.isSolved),o(),pe(" ",n+1," ")}}var j=class t{quizAttemptStore=p(C);onClick(i){this.quizAttemptStore.setCurrentQuestionIndex(i)}static \u0275fac=function(e){return new(e||t)};static \u0275cmp=c({type:t,selectors:[["app-questions-navigator"]],decls:16,vars:0,consts:[["aria-label","Question navigator",1,"navigator-card"],[1,"navigator-grid"],["type","button",3,"is-current","is-flagged","is-solved"],["aria-label","Question status legend",1,"legend"],[1,"dot","answered"],[1,"dot","unanswered"],[1,"dot","flagged"],["type","button",3,"click"]],template:function(e,n){e&1&&(b(0,"section",0)(1,"h2"),l(2,"Question Navigator"),_(),b(3,"div",1),ae(4,De,2,7,"button",2,oe),_(),b(6,"ul",3)(7,"li"),E(8,"span",4),l(9,"Answered"),_(),b(10,"li"),E(11,"span",5),l(12,"Unanswered"),_(),b(13,"li"),E(14,"span",6),l(15,"Flagged"),_()()()),e&2&&(o(4),se(n.quizAttemptStore.quizQuestions()))},styles:["[_nghost-%COMP%]{display:block}.navigator-card[_ngcontent-%COMP%]{display:grid;gap:.75rem;padding:1rem;border:1px solid var(--clr-gray-300);border-radius:var(--radius-md);background:var(--clr-white)}h2[_ngcontent-%COMP%]{margin:0;font-size:1rem}.navigator-grid[_ngcontent-%COMP%]{display:grid;grid-template-columns:repeat(5,minmax(2rem,1fr));gap:.5rem}@media(width<480px){.navigator-grid[_ngcontent-%COMP%]{grid-template-columns:repeat(4,minmax(2rem,1fr))}}@media(width<380px){.navigator-grid[_ngcontent-%COMP%]{grid-template-columns:repeat(3,minmax(2rem,1fr))}}button[_ngcontent-%COMP%]{min-height:2rem;border:1px solid var(--clr-gray-300);border-radius:.625rem;background:var(--clr-gray-100);color:var(--clr-gray-500);font-weight:600}button.is-solved[_ngcontent-%COMP%]{background:var(--clr-green-400);color:var(--clr-white);border-color:var(--clr-green-400)}button.is-flagged[_ngcontent-%COMP%]{background:var(--clr-amber-100);color:var(--clr-red-500);border-color:var(--clr-amber-100)}button.is-current[_ngcontent-%COMP%]{border:2px solid var(--clr-green-400);background:var(--clr-white);color:var(--clr-green-400)}.legend[_ngcontent-%COMP%]{display:grid;gap:.25rem;margin:0;padding:0;color:var(--clr-gray-600);font-size:.875rem;list-style:none}.legend[_ngcontent-%COMP%]   li[_ngcontent-%COMP%]{display:flex;align-items:center;gap:.5rem}.dot[_ngcontent-%COMP%]{display:inline-block;width:.625rem;height:.625rem;border-radius:var(--radius-sm)}.answered[_ngcontent-%COMP%]{background:var(--clr-green-400)}.unanswered[_ngcontent-%COMP%]{background:var(--clr-gray-300)}.flagged[_ngcontent-%COMP%]{background:var(--clr-amber-100)}"],changeDetection:0})};var Ae=`
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
`;var Be=["content"],Ne=t=>({$implicit:t});function Fe(t,i){if(t&1&&(a(0,"div"),l(1),s()),t&2){let e=u(2);I("display",e.value!=null&&e.value!==0?"flex":"none"),o(),q("",e.value,"",e.unit)}}function Ve(t,i){t&1&&M(0)}function Le(t,i){if(t&1&&(a(0,"div",2)(1,"div",2),J(2,Fe,2,4,"div",3)(3,Ve,1,0,"ng-container",4),s()()),t&2){let e=u();T(e.cn(e.cx("value"),e.valueStyleClass)),I("width",e.value+"%")("display","flex")("background",e.color),m("pBind",e.ptm("value")),P("data-p",e.dataP),o(),T(e.cx("label")),m("pBind",e.ptm("label")),P("data-p",e.dataP),o(),m("ngIf",e.showValue&&!e.contentTemplate&&!e._contentTemplate),o(),m("ngTemplateOutlet",e.contentTemplate||e._contentTemplate)("ngTemplateOutletContext",N(17,Ne,e.value))}}function Re(t,i){if(t&1&&g(0,"div",2),t&2){let e=u();T(e.cn(e.cx("value"),e.valueStyleClass)),I("background",e.color),m("pBind",e.ptm("value")),P("data-p",e.dataP)}}var He={root:({instance:t})=>["p-progressbar p-component",{"p-progressbar-determinate":t.mode=="determinate","p-progressbar-indeterminate":t.mode=="indeterminate"}],value:"p-progressbar-value",label:"p-progressbar-label"},Pe=(()=>{class t extends he{name="progressbar";style=Ae;classes=He;static \u0275fac=(()=>{let e;return function(r){return(e||(e=W(t)))(r||t)}})();static \u0275prov=ee({token:t,factory:t.\u0275fac})}return t})();var Te=new te("PROGRESSBAR_INSTANCE"),ke=(()=>{class t extends Se{componentName="ProgressBar";$pcProgressBar=p(Te,{optional:!0,skipSelf:!0})??void 0;bindDirectiveInstance=p(k,{self:!0});value;showValue=!0;styleClass;valueStyleClass;unit="%";mode="determinate";color;contentTemplate;onAfterViewChecked(){this.bindDirectiveInstance.setAttrs(this.ptms(["host","root"]))}_componentStyle=p(Pe);templates;_contentTemplate;onAfterContentInit(){this.templates?.forEach(e=>{e.getType()==="content"?this._contentTemplate=e.template:this._contentTemplate=e.template})}get dataP(){return this.cn({determinate:this.mode==="determinate",indeterminate:this.mode==="indeterminate"})}static \u0275fac=(()=>{let e;return function(r){return(e||(e=W(t)))(r||t)}})();static \u0275cmp=c({type:t,selectors:[["p-progressBar"],["p-progressbar"],["p-progress-bar"]],contentQueries:function(n,r,d){if(n&1&&de(d,Be,4)(d,ye,4),n&2){let S;K(S=X())&&(r.contentTemplate=S.first),K(S=X())&&(r.templates=S)}},hostAttrs:["role","progressbar"],hostVars:7,hostBindings:function(n,r){n&2&&(P("aria-valuemin",0)("aria-valuenow",r.value)("aria-valuemax",100)("aria-level",r.value+r.unit)("data-p",r.dataP),T(r.cn(r.cx("root"),r.styleClass)))},inputs:{value:[2,"value","value",ce],showValue:[2,"showValue","showValue",ue],styleClass:"styleClass",valueStyleClass:"valueStyleClass",unit:"unit",mode:"mode",color:"color"},features:[B([Pe,{provide:Te,useExisting:t},{provide:ze,useExisting:t}]),ie([k]),re],decls:2,vars:2,consts:[[3,"class","pBind","width","display","background",4,"ngIf"],[3,"class","pBind","background",4,"ngIf"],[3,"pBind"],[3,"display",4,"ngIf"],[4,"ngTemplateOutlet","ngTemplateOutletContext"]],template:function(n,r){n&1&&J(0,Le,4,19,"div",0)(1,Re,1,6,"div",1),n&2&&(m("ngIf",r.mode==="determinate"),o(),m("ngIf",r.mode==="indeterminate"))},dependencies:[ve,ge,fe,Ce,k],encapsulation:2,changeDetection:0})}return t})();var G=class t{quizAttemptStore=p(C);progressValue=Q(()=>{let i=this.quizAttemptStore.numberOfQuestions();return i===0?0:Math.round(this.quizAttemptStore.numberOfSolvedQuestions()/i*100)});static \u0275fac=function(e){return new(e||t)};static \u0275cmp=c({type:t,selectors:[["app-questions-progress-bar"]],decls:4,vars:4,consts:[["aria-label","Quiz progress",1,"progress-card"],["aria-label","Solved questions progress",1,"quiz-progress",3,"value","showValue"],[1,"progress-summary"]],template:function(e,n){e&1&&(a(0,"section",0),g(1,"p-progressbar",1),a(2,"p",2),l(3),s()()),e&2&&(o(),m("value",n.progressValue())("showValue",!1),o(2),q(" ",n.quizAttemptStore.numberOfSolvedQuestions()," of ",n.quizAttemptStore.numberOfQuestions()," answered "))},dependencies:[ke],styles:["[_nghost-%COMP%]{display:block}.progress-card[_ngcontent-%COMP%]{display:grid;gap:.5rem;padding:1rem;border:1px solid var(--clr-gray-300);border-radius:var(--radius-md);background:var(--clr-white)}.quiz-progress[_ngcontent-%COMP%]{height:1rem;border-radius:var(--radius-lg);background:var(--clr-gray-200)}.quiz-progress[_ngcontent-%COMP%]   .p-progressbar-value[_ngcontent-%COMP%]{border-radius:var(--radius-lg);background:var(--gradient-main)}.progress-summary[_ngcontent-%COMP%]{margin:0;color:var(--clr-gray-600);font-size:var(--fs-300);font-weight:600;text-align:center}"],changeDetection:0})};var Y=class t{quizAttemptStore=p(C);remainingTime=Q(()=>{let i=this.quizAttemptStore.remaningSeconds(),e=Math.floor(i/60),n=i%60;return`${e.toString().padStart(2,"0")}:${n.toString().padStart(2,"0")}`});static \u0275fac=function(e){return new(e||t)};static \u0275cmp=c({type:t,selectors:[["app-quiz-attempt-header"]],decls:11,vars:6,consts:[[1,"attempt-header"],["aria-label","Quiz status",1,"attempt-meta"],[1,"chip"]],template:function(e,n){e&1&&(b(0,"header",0)(1,"div")(2,"h1"),l(3),_(),b(4,"p"),l(5),_()(),b(6,"div",1)(7,"span",2),l(8),_(),b(9,"span",2),l(10),_()()()),e&2&&(o(3),x(n.quizAttemptStore.quizTitle()),o(2),q(" Question ",n.quizAttemptStore.currentQuestionIndex()," of ",n.quizAttemptStore.numberOfQuestions()," "),o(3),q("",n.quizAttemptStore.numberOfSolvedQuestions(),"/",n.quizAttemptStore.numberOfQuestions()),o(2),x(n.remainingTime()))},styles:["[_nghost-%COMP%]{display:block}.attempt-header[_ngcontent-%COMP%]{display:flex;align-items:center;justify-content:space-between;gap:.75rem;padding:1rem;border:1px solid var(--clr-gray-300);border-radius:var(--radius-md);background:var(--clr-white)}h1[_ngcontent-%COMP%]{margin:0;font-size:1.25rem}p[_ngcontent-%COMP%]{margin:.25rem 0 0;color:var(--clr-gray-600);font-size:.875rem}.attempt-meta[_ngcontent-%COMP%]{display:flex;justify-content:end;flex-wrap:wrap;gap:.5rem}.chip[_ngcontent-%COMP%]{padding:.35rem .65rem;border:1px solid var(--clr-gray-300);border-radius:var(--radius-sm);color:var(--clr-gray-600);font-size:.875rem;font-weight:600}@media(width<=40rem){.attempt-header[_ngcontent-%COMP%]{align-items:flex-start;flex-direction:column}}"],changeDetection:0})};var $=class t{seeResults=me();static \u0275fac=function(e){return new(e||t)};static \u0275cmp=c({type:t,selectors:[["app-quiz-finished-message"]],outputs:{seeResults:"seeResults"},decls:8,vars:0,consts:[[1,"quiz-completed-card"],[1,"quiz-completed-card__icon"],[1,"fa-solid","fa-circle-check"],[1,"quiz-completed-card__title"],[1,"quiz-completed-card__message"],["label","See Results","severity","success","type","button",3,"onClick"]],template:function(e,n){e&1&&(a(0,"div",0)(1,"div",1),g(2,"i",2),s(),a(3,"h2",3),l(4,"Quiz Completed!"),s(),a(5,"p",4),l(6," You Have Compelete the Quiz Take a Rest Before you See you results \u{1F609} "),s(),a(7,"p-button",5),y("onClick",function(){return n.seeResults.emit()}),s()())},dependencies:[L],styles:["[_nghost-%COMP%]{display:block}.quiz-completed-card[_ngcontent-%COMP%]{display:grid;gap:1.5rem;width:100%;max-width:32rem;margin:4rem auto;padding:3.5rem 2rem;border:1px solid var(--clr-gray-200);border-radius:var(--radius-lg);background-color:var(--clr-white);text-align:center;animation:_ngcontent-%COMP%_fade-in-up .4s cubic-bezier(.16,1,.3,1);box-shadow:0 10px 15px -3px #0000000d,0 4px 6px -2px #00000005;place-items:center;align-content:center}.quiz-completed-card__icon[_ngcontent-%COMP%]{color:var(--clr-green-400);font-size:3.5rem;animation:_ngcontent-%COMP%_scale-in .5s cubic-bezier(.16,1,.3,1)}.quiz-completed-card__title[_ngcontent-%COMP%]{margin:0;color:var(--clr-blue-900);font-size:1.8rem;font-weight:700}.quiz-completed-card__message[_ngcontent-%COMP%]{margin:0;color:var(--clr-gray-600);font-size:1.1rem;line-height:1.6}@keyframes _ngcontent-%COMP%_fade-in-up{0%{transform:translateY(20px);opacity:0}to{transform:translateY(0);opacity:1}}@keyframes _ngcontent-%COMP%_scale-in{0%{transform:scale(.5);opacity:0}to{transform:scale(1);opacity:1}}"],changeDetection:0})};var je=t=>({question:t});function Ge(t,i){t&1&&(a(0,"div",1),g(1,"p-progress-spinner",4),s())}function Ye(t,i){t&1&&(a(0,"app-operation-failed")(1,"p"),l(2),s()()),t&2&&(o(2),x(i))}function $e(t,i){if(t&1){let e=w();a(0,"app-quiz-finished-message",5),y("seeResults",function(){f(e);let r=u();return v(r.goToResults())}),s()}}function Ue(t,i){t&1&&(a(0,"app-operation-failed")(1,"p"),l(2),s()()),t&2&&(o(2),x(i))}function We(t,i){t&1&&(a(0,"app-operation-failed")(1,"p"),l(2),s()()),t&2&&(o(2),x(i))}function Je(t,i){t&1&&(a(0,"app-operation-failed")(1,"p"),l(2),s()()),t&2&&(o(2),x(i))}function Ke(t,i){if(t&1){let e=w();g(0,"app-quiz-attempt-header"),h(1,Ue,3,1,"app-operation-failed"),h(2,We,3,1,"app-operation-failed"),h(3,Je,3,1,"app-operation-failed"),a(4,"div",6)(5,"div",7),g(6,"app-question-attempt-header",8),M(7,9),a(8,"app-navigation-buttons",10),y("previousButtonClicked",function(){f(e);let r=u();return v(r.quizAttemptStore.GoToPreviousQuestion())})("nextButtonClicked",function(){f(e);let r=u();return v(r.quizAttemptStore.GoToNextQuestion())}),s(),a(9,"p-button",11),y("onClick",function(){f(e);let r=u();return v(r.quizAttemptStore.saveCurrentAnswer())}),s()(),a(10,"aside",12),g(11,"app-questions-navigator")(12,"app-questions-progress-bar"),a(13,"p-button",13),y("onClick",function(){f(e);let r=u();return v(r.onSubmitQuiz())}),s()()()}if(t&2){let e,n,r,d=u();o(),z((e=d.quizAttemptStore.error()("submit"))?1:-1,e),o(),z((n=d.quizAttemptStore.error()("submit-answer"))?2:-1,n),o(),z((r=d.quizAttemptStore.error()("start"))?3:-1,r);let S=d.quizAttemptStore.quizQuestions()[d.quizAttemptStore.currentQuestionIndex()];o(3),m("questionType",S.type),o(),m("ngComponentOutlet",d.questionMapperService.getSuitableQuestionAttemptComponent(S.type))("ngComponentOutletInputs",N(13,je,S)),o(),m("canGoPrevious",d.quizAttemptStore.canGoPrevious())("canGoNext",d.quizAttemptStore.canGoNext()),o(),m("disabled",!d.quizAttemptStore.currentAnswerDraft()||d.quizAttemptStore.quizTimeOut())("loading",d.quizAttemptStore.isPending()("submit-answer"))("label",d.savedLabel()??"Save Answer"),o(4),m("fluid",!0)("loading",d.quizAttemptStore.isPending()("submit"))}}function Xe(t,i){if(t&1){let e=w();a(0,"app-confirm-action-modal",14),y("confirmed",function(){f(e);let r=u();return v(r.onLeave(!0))})("cancelled",function(){f(e);let r=u();return v(r.onLeave(!1))}),s()}}function Ze(t,i){if(t&1){let e=w();a(0,"app-confirm-action-modal",15),y("confirmed",function(){f(e);let r=u();return v(r.onConfirmSubmit())})("cancelled",function(){f(e);let r=u();return v(r.showSubmitConfirmModal.set(!1))}),s()}}var Oe=class t{questionMapperService=p(R);quizId=F.required();quizAttemptStore=p(C);router=p(_e);route=p(be);showLeaveConfirmModal=U(!1);showSubmitConfirmModal=U(!1);resolveLeave=null;savedLabel=Q(()=>{let i=this.quizAttemptStore.lastSavedAt();return!i||Date.now()-i>3e3?null:"\u2713 Saved"});attemptId=xe(this.route.queryParamMap.pipe(Z(i=>i.get("attemptId"))));ngOnInit(){this.quizAttemptStore.load({quizId:this.quizId(),attemptId:this.attemptId()})}unloadNotification(i){this.isQuizInProgress()&&i.preventDefault()}canDeactivate(){return this.isQuizInProgress()?(this.showLeaveConfirmModal.set(!0),new Promise(i=>{this.resolveLeave=i})):!0}onLeave(i){this.showLeaveConfirmModal.set(!1),this.resolveLeave?.(i),this.resolveLeave=null}onSubmitQuiz(){this.showSubmitConfirmModal.set(!0)}onConfirmSubmit(){this.showSubmitConfirmModal.set(!1),this.quizAttemptStore.completeAttempt()}goToResults(){this.router.navigate(["/student/results"])}isQuizInProgress(){let i=this.quizAttemptStore,e=!i.isPending()("load")&&!i.error()("load"),n=i.isFulfilled()("submit");return e&&!n}static \u0275fac=function(e){return new(e||t)};static \u0275cmp=c({type:t,selectors:[["app-quiz-attempt"]],hostBindings:function(e,n){e&1&&y("beforeunload",function(d){return n.unloadNotification(d)},ne)},inputs:{quizId:[1,"quizId"]},features:[B([C])],decls:7,vars:3,consts:[["aria-label","Quiz attempt layout",1,"attempt-layout"],[1,"spinner-container"],["title","Leave Quiz","warningMessage","Are you sure you want to leave? Your progress will be saved.","confirmationPhrase","leave","confirmButtonText","I understand, leave","variant","danger"],["title","Submit Quiz","warningMessage","Are you sure you want to submit your quiz? You will not be able to edit your answers after this.","confirmationPhrase","submit","confirmButtonText","Yes, Submit Quiz","variant","success"],["ariaLabel","Loading quiz attempt"],[3,"seeResults"],[1,"attempt-main"],["aria-label","Question area",1,"question-column"],[3,"questionType"],[3,"ngComponentOutlet","ngComponentOutletInputs"],["ariaLabel","Question navigation",3,"previousButtonClicked","nextButtonClicked","canGoPrevious","canGoNext"],["severity","success","type","button",3,"onClick","disabled","loading","label"],["aria-label","Quiz tools",1,"sidebar-column"],["label","Submit Quiz","severity","danger","type","button",3,"onClick","fluid","loading"],["title","Leave Quiz","warningMessage","Are you sure you want to leave? Your progress will be saved.","confirmationPhrase","leave","confirmButtonText","I understand, leave","variant","danger",3,"confirmed","cancelled"],["title","Submit Quiz","warningMessage","Are you sure you want to submit your quiz? You will not be able to edit your answers after this.","confirmationPhrase","submit","confirmButtonText","Yes, Submit Quiz","variant","success",3,"confirmed","cancelled"]],template:function(e,n){if(e&1&&(a(0,"section",0),h(1,Ge,2,0,"div",1)(2,Ye,3,1,"app-operation-failed")(3,$e,1,0,"app-quiz-finished-message")(4,Ke,14,15),h(5,Xe,1,0,"app-confirm-action-modal",2),h(6,Ze,1,0,"app-confirm-action-modal",3),s()),e&2){let r;o(),z(n.quizAttemptStore.isPending()("load")?1:(r=n.quizAttemptStore.error()("load"))?2:n.quizAttemptStore.isFulfilled()("submit")?3:4,r),o(4),z(n.showLeaveConfirmModal()?5:-1),o(),z(n.showSubmitConfirmModal()?6:-1)}},dependencies:[Y,j,Qe,H,V,G,we,qe,L,$,Me],styles:["[_nghost-%COMP%]{display:block;padding:1rem}.attempt-layout[_ngcontent-%COMP%]{display:grid;gap:1rem;width:min(100%,70rem);margin:0 auto}.attempt-main[_ngcontent-%COMP%]{display:grid;align-items:start;gap:1rem;grid-template-columns:2fr 1fr}.question-column[_ngcontent-%COMP%], .sidebar-column[_ngcontent-%COMP%]{display:grid;gap:1rem}.spinner-container[_ngcontent-%COMP%]{display:flex;align-items:center;justify-content:center;min-height:20rem}@media(width<=64rem){.attempt-main[_ngcontent-%COMP%]{grid-template-columns:1fr}}"],changeDetection:0})};export{Oe as QuizAttempt};
