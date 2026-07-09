import{a as Me}from"./chunk-AZQWJEEV.js";import{a as Qe}from"./chunk-RF2XKIXC.js";import{a as C,b as H}from"./chunk-6H52Z4C3.js";import"./chunk-6Q5UIQRJ.js";import"./chunk-63JTRK66.js";import"./chunk-2JHMGDK6.js";import"./chunk-EZUW2TZA.js";import"./chunk-XSEFHWRE.js";import"./chunk-GL5RWWNV.js";import"./chunk-4CLXHCO4.js";import"./chunk-EGPUZ3IU.js";import"./chunk-CW4C2QPI.js";import{a as qe}from"./chunk-5QURMJHP.js";import{a as we}from"./chunk-RH3JMYYT.js";import"./chunk-PVQRTERY.js";import{a as R}from"./chunk-NTO3YJKF.js";import{b as be,d as _e}from"./chunk-FSOC6KW5.js";import{c as xe}from"./chunk-CJEG3NBD.js";import"./chunk-TCTXBJ4R.js";import{da as ye,ea as Ce,ha as he,ja as ze,ka as Se,la as k}from"./chunk-67O4C7MM.js";import{h as L,j as ge,l as fe,o as ve}from"./chunk-IXU7L534.js";import{Ab as u,Ac as ce,Db as me,Fb as X,Gb as Z,Kb as I,Lb as D,M as te,Ma as c,Nb as T,Ob as s,P as ne,Pb as w,Qa as re,Qb as B,R as m,Ra as oe,Rb as q,Sa as K,W as f,X as v,Yb as N,_b as F,bb as P,eb as h,fb as z,gb as ae,ha as W,ib as se,jb as le,kb as p,la as J,lb as a,mb as l,mc as Q,n as ee,nb as g,ob as b,pb as _,qb as E,rc as pe,sc as V,tb as M,ub as x,wa as ie,xa as o,yb as y,zb as de,zc as ue}from"./chunk-N3IKTFHA.js";import"./chunk-HIUAR4UN.js";function Ee(t,n){t&1&&(a(0,"span"),s(1,"Unflag"),l())}function Ie(t,n){t&1&&(a(0,"span"),s(1,"Flag"),l())}var j=class t{mapperService=m(H);quizAttemptStore=m(C);questionType=V.required();onClickFlag(){this.quizAttemptStore.changeFlagStatusForTheCurrentQuestion()}static \u0275fac=function(e){return new(e||t)};static \u0275cmp=c({type:t,selectors:[["app-question-attempt-header"]],inputs:{questionType:[1,"questionType"]},decls:6,vars:4,consts:[[1,"question-attempt-header"],[3,"ngComponentOutlet"],["type","button","aria-label","Flag question",1,"flag","btn",3,"click"],["aria-hidden","true",1,"fa-solid","fa-circle-exclamation"]],template:function(e,i){e&1&&(a(0,"header",0),M(1,1),a(2,"button",2),y("click",function(){return i.onClickFlag()}),g(3,"i",3),h(4,Ee,2,0,"span")(5,Ie,2,0,"span"),l()()),e&2&&(o(),p("ngComponentOutlet",i.mapperService.getSuitableQuestionTag(i.questionType())),o(),D("flagged",i.quizAttemptStore.isCurrentQuestionFlagged()),o(2),z(i.quizAttemptStore.isCurrentQuestionFlagged()?4:5))},dependencies:[L],styles:[".question-attempt-header[_ngcontent-%COMP%]{display:flex;align-items:center;justify-content:space-between;gap:.75rem}.flag[_ngcontent-%COMP%]{display:inline-flex;align-items:center;gap:.5rem;padding:.4rem .85rem;border:1px solid var(--clr-gray-300);border-radius:var(--radius-lg);background-color:var(--clr-gray-100);color:var(--clr-gray-600);font-size:var(--fs-600);font-weight:500;line-height:1}.flagged[_ngcontent-%COMP%]{border-color:var(--clr-amber-100);background-color:var(--clr-amber-100);color:var(--clr-red-500)}"],changeDetection:0})};function De(t,n){if(t&1){let e=x();b(0,"button",7),de("click",function(){let r=f(e).$index,d=u();return v(d.onClick(r))}),s(1),_()}if(t&2){let e=n.$implicit,i=n.$index,r=u();D("is-current",i===r.quizAttemptStore.currentQuestionIndex())("is-flagged",e.isFlagged)("is-solved",e.isSolved),o(),B(" ",i+1," ")}}var G=class t{quizAttemptStore=m(C);onClick(n){this.quizAttemptStore.setCurrentQuestionIndex(n)}static \u0275fac=function(e){return new(e||t)};static \u0275cmp=c({type:t,selectors:[["app-questions-navigator"]],decls:16,vars:0,consts:[["aria-label","Question navigator",1,"navigator-card"],[1,"navigator-grid"],["type","button",3,"is-current","is-flagged","is-solved"],["aria-label","Question status legend",1,"legend"],[1,"dot","answered"],[1,"dot","unanswered"],[1,"dot","flagged"],["type","button",3,"click"]],template:function(e,i){e&1&&(b(0,"section",0)(1,"h2"),s(2,"Question Navigator"),_(),b(3,"div",1),se(4,De,2,7,"button",2,ae),_(),b(6,"ul",3)(7,"li"),E(8,"span",4),s(9,"Answered"),_(),b(10,"li"),E(11,"span",5),s(12,"Unanswered"),_(),b(13,"li"),E(14,"span",6),s(15,"Flagged"),_()()()),e&2&&(o(4),le(i.quizAttemptStore.quizQuestions()))},styles:["[_nghost-%COMP%]{display:block}.navigator-card[_ngcontent-%COMP%]{display:grid;gap:.75rem;padding:1rem;border:1px solid var(--clr-gray-300);border-radius:var(--radius-md);background:var(--clr-white)}h2[_ngcontent-%COMP%]{margin:0;font-size:1rem}.navigator-grid[_ngcontent-%COMP%]{display:grid;grid-template-columns:repeat(5,minmax(2rem,1fr));gap:.5rem}button[_ngcontent-%COMP%]{border:1px solid var(--clr-gray-300);border-radius:.625rem;min-height:2rem;background:var(--clr-gray-100);font-weight:600;color:var(--clr-gray-500)}button.is-solved[_ngcontent-%COMP%]{background:var(--clr-green-400);color:var(--clr-white);border-color:var(--clr-green-400)}button.is-flagged[_ngcontent-%COMP%]{background:var(--clr-amber-100);color:var(--clr-red-500);border-color:var(--clr-amber-100)}button.is-current[_ngcontent-%COMP%]{background:var(--clr-white);color:var(--clr-green-400);border:2px solid var(--clr-green-400)}.legend[_ngcontent-%COMP%]{list-style:none;padding:0;margin:0;display:grid;gap:.25rem;font-size:.875rem;color:var(--clr-gray-600)}.legend[_ngcontent-%COMP%]   li[_ngcontent-%COMP%]{display:flex;align-items:center;gap:.5rem}.dot[_ngcontent-%COMP%]{width:.625rem;height:.625rem;border-radius:var(--radius-sm);display:inline-block}.answered[_ngcontent-%COMP%]{background:var(--clr-green-400)}.unanswered[_ngcontent-%COMP%]{background:var(--clr-gray-300)}.flagged[_ngcontent-%COMP%]{background:var(--clr-amber-100)}"],changeDetection:0})};var Ae=`
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
`;var Be=["content"],Ne=t=>({$implicit:t});function Fe(t,n){if(t&1&&(a(0,"div"),s(1),l()),t&2){let e=u(2);I("display",e.value!=null&&e.value!==0?"flex":"none"),o(),q("",e.value,"",e.unit)}}function Ve(t,n){t&1&&M(0)}function Le(t,n){if(t&1&&(a(0,"div",2)(1,"div",2),K(2,Fe,2,4,"div",3)(3,Ve,1,0,"ng-container",4),l()()),t&2){let e=u();T(e.cn(e.cx("value"),e.valueStyleClass)),I("width",e.value+"%")("display","flex")("background",e.color),p("pBind",e.ptm("value")),P("data-p",e.dataP),o(),T(e.cx("label")),p("pBind",e.ptm("label")),P("data-p",e.dataP),o(),p("ngIf",e.showValue&&!e.contentTemplate&&!e._contentTemplate),o(),p("ngTemplateOutlet",e.contentTemplate||e._contentTemplate)("ngTemplateOutletContext",F(17,Ne,e.value))}}function Re(t,n){if(t&1&&g(0,"div",2),t&2){let e=u();T(e.cn(e.cx("value"),e.valueStyleClass)),I("background",e.color),p("pBind",e.ptm("value")),P("data-p",e.dataP)}}var He={root:({instance:t})=>["p-progressbar p-component",{"p-progressbar-determinate":t.mode=="determinate","p-progressbar-indeterminate":t.mode=="indeterminate"}],value:"p-progressbar-value",label:"p-progressbar-label"},Pe=(()=>{class t extends he{name="progressbar";style=Ae;classes=He;static \u0275fac=(()=>{let e;return function(r){return(e||(e=J(t)))(r||t)}})();static \u0275prov=te({token:t,factory:t.\u0275fac})}return t})();var Te=new ne("PROGRESSBAR_INSTANCE"),ke=(()=>{class t extends Se{componentName="ProgressBar";$pcProgressBar=m(Te,{optional:!0,skipSelf:!0})??void 0;bindDirectiveInstance=m(k,{self:!0});value;showValue=!0;styleClass;valueStyleClass;unit="%";mode="determinate";color;contentTemplate;onAfterViewChecked(){this.bindDirectiveInstance.setAttrs(this.ptms(["host","root"]))}_componentStyle=m(Pe);templates;_contentTemplate;onAfterContentInit(){this.templates?.forEach(e=>{e.getType()==="content"?this._contentTemplate=e.template:this._contentTemplate=e.template})}get dataP(){return this.cn({determinate:this.mode==="determinate",indeterminate:this.mode==="indeterminate"})}static \u0275fac=(()=>{let e;return function(r){return(e||(e=J(t)))(r||t)}})();static \u0275cmp=c({type:t,selectors:[["p-progressBar"],["p-progressbar"],["p-progress-bar"]],contentQueries:function(i,r,d){if(i&1&&me(d,Be,4)(d,ye,4),i&2){let S;X(S=Z())&&(r.contentTemplate=S.first),X(S=Z())&&(r.templates=S)}},hostAttrs:["role","progressbar"],hostVars:7,hostBindings:function(i,r){i&2&&(P("aria-valuemin",0)("aria-valuenow",r.value)("aria-valuemax",100)("aria-level",r.value+r.unit)("data-p",r.dataP),T(r.cn(r.cx("root"),r.styleClass)))},inputs:{value:[2,"value","value",ce],showValue:[2,"showValue","showValue",ue],styleClass:"styleClass",valueStyleClass:"valueStyleClass",unit:"unit",mode:"mode",color:"color"},features:[N([Pe,{provide:Te,useExisting:t},{provide:ze,useExisting:t}]),re([k]),oe],decls:2,vars:2,consts:[[3,"class","pBind","width","display","background",4,"ngIf"],[3,"class","pBind","background",4,"ngIf"],[3,"pBind"],[3,"display",4,"ngIf"],[4,"ngTemplateOutlet","ngTemplateOutletContext"]],template:function(i,r){i&1&&K(0,Le,4,19,"div",0)(1,Re,1,6,"div",1),i&2&&(p("ngIf",r.mode==="determinate"),o(),p("ngIf",r.mode==="indeterminate"))},dependencies:[ve,ge,fe,Ce,k],encapsulation:2,changeDetection:0})}return t})();var Y=class t{quizAttemptStore=m(C);progressValue=Q(()=>{let n=this.quizAttemptStore.numberOfQuestions();return n===0?0:Math.round(this.quizAttemptStore.numberOfSolvedQuestions()/n*100)});static \u0275fac=function(e){return new(e||t)};static \u0275cmp=c({type:t,selectors:[["app-questions-progress-bar"]],decls:4,vars:4,consts:[["aria-label","Quiz progress",1,"progress-card"],["aria-label","Solved questions progress",1,"quiz-progress",3,"value","showValue"],[1,"progress-summary"]],template:function(e,i){e&1&&(a(0,"section",0),g(1,"p-progressbar",1),a(2,"p",2),s(3),l()()),e&2&&(o(),p("value",i.progressValue())("showValue",!1),o(2),q(" ",i.quizAttemptStore.numberOfSolvedQuestions()," of ",i.quizAttemptStore.numberOfQuestions()," answered "))},dependencies:[ke],styles:["[_nghost-%COMP%]{display:block}.progress-card[_ngcontent-%COMP%]{display:grid;gap:.5rem;padding:1rem;border:1px solid var(--clr-gray-300);border-radius:var(--radius-md);background:var(--clr-white)}.quiz-progress[_ngcontent-%COMP%]{height:1rem;border-radius:var(--radius-lg);background:var(--clr-gray-200)}.quiz-progress[_ngcontent-%COMP%]   .p-progressbar-value[_ngcontent-%COMP%]{border-radius:var(--radius-lg);background:var(--gradient-main)}.progress-summary[_ngcontent-%COMP%]{margin:0;text-align:center;font-size:var(--fs-300);color:var(--clr-gray-600);font-weight:600}"],changeDetection:0})};var $=class t{quizAttemptStore=m(C);remainingTime=Q(()=>{let n=this.quizAttemptStore.remaningSeconds(),e=Math.floor(n/60),i=n%60;return`${e.toString().padStart(2,"0")}:${i.toString().padStart(2,"0")}`});static \u0275fac=function(e){return new(e||t)};static \u0275cmp=c({type:t,selectors:[["app-quiz-attempt-header"]],decls:11,vars:6,consts:[[1,"attempt-header"],["aria-label","Quiz status",1,"attempt-meta"],[1,"chip"]],template:function(e,i){e&1&&(b(0,"header",0)(1,"div")(2,"h1"),s(3),_(),b(4,"p"),s(5),_()(),b(6,"div",1)(7,"span",2),s(8),_(),b(9,"span",2),s(10),_()()()),e&2&&(o(3),w(i.quizAttemptStore.quizTitle()),o(2),q(" Question ",i.quizAttemptStore.currentQuestionIndex()," of ",i.quizAttemptStore.numberOfQuestions()," "),o(3),q("",i.quizAttemptStore.numberOfSolvedQuestions(),"/",i.quizAttemptStore.numberOfQuestions()),o(2),w(i.remainingTime()))},styles:["[_nghost-%COMP%]{display:block}.attempt-header[_ngcontent-%COMP%]{display:flex;justify-content:space-between;align-items:center;gap:.75rem;padding:1rem;border:1px solid var(--clr-gray-300);border-radius:var(--radius-md);background:var(--clr-white)}h1[_ngcontent-%COMP%]{margin:0;font-size:1.25rem}p[_ngcontent-%COMP%]{margin:.25rem 0 0;color:var(--clr-gray-600);font-size:.875rem}.attempt-meta[_ngcontent-%COMP%]{display:flex;gap:.5rem;flex-wrap:wrap;justify-content:end}.chip[_ngcontent-%COMP%]{padding:.35rem .65rem;border:1px solid var(--clr-gray-300);border-radius:var(--radius-sm);font-size:.875rem;font-weight:600;color:var(--clr-gray-600)}@media(width<=40rem){.attempt-header[_ngcontent-%COMP%]{flex-direction:column;align-items:flex-start}}"],changeDetection:0})};var U=class t{seeResults=pe();static \u0275fac=function(e){return new(e||t)};static \u0275cmp=c({type:t,selectors:[["app-quiz-finished-message"]],outputs:{seeResults:"seeResults"},decls:9,vars:0,consts:[[1,"quiz-completed-card"],[1,"quiz-completed-card__icon"],[1,"fa-solid","fa-circle-check"],[1,"quiz-completed-card__title"],[1,"quiz-completed-card__message"],["appButton","","variant","green","type","button",1,"quiz-completed-card__btn",3,"click"]],template:function(e,i){e&1&&(a(0,"div",0)(1,"div",1),g(2,"i",2),l(),a(3,"h2",3),s(4,"Quiz Completed!"),l(),a(5,"p",4),s(6," You Have Compelete the Quiz Take a Rest Before you See you results \u{1F609} "),l(),a(7,"button",5),y("click",function(){return i.seeResults.emit()}),s(8," See Results "),l()())},dependencies:[R],styles:["[_nghost-%COMP%]{display:block}.quiz-completed-card[_ngcontent-%COMP%]{display:grid;place-items:center;align-content:center;gap:1.5rem;padding:3.5rem 2rem;background-color:var(--clr-white);border:1px solid var(--clr-gray-200);border-radius:var(--radius-lg);box-shadow:0 10px 15px -3px #0000000d,0 4px 6px -2px #00000005;max-width:32rem;width:100%;margin:4rem auto;text-align:center;animation:_ngcontent-%COMP%_fadeInUp .4s cubic-bezier(.16,1,.3,1)}.quiz-completed-card__icon[_ngcontent-%COMP%]{font-size:3.5rem;color:var(--clr-green-400);animation:_ngcontent-%COMP%_scaleIn .5s cubic-bezier(.16,1,.3,1)}.quiz-completed-card__title[_ngcontent-%COMP%]{font-size:1.8rem;font-weight:700;color:var(--clr-blue-900);margin:0}.quiz-completed-card__message[_ngcontent-%COMP%]{font-size:1.1rem;line-height:1.6;color:var(--clr-gray-600);margin:0}.quiz-completed-card__btn[_ngcontent-%COMP%]{min-width:12rem}@keyframes _ngcontent-%COMP%_fadeInUp{0%{opacity:0;transform:translateY(20px)}to{opacity:1;transform:translateY(0)}}@keyframes _ngcontent-%COMP%_scaleIn{0%{opacity:0;transform:scale(.5)}to{opacity:1;transform:scale(1)}}"],changeDetection:0})};var je=t=>({question:t});function Ge(t,n){t&1&&(a(0,"div",1),g(1,"p-progress-spinner",4),l())}function Ye(t,n){t&1&&(a(0,"app-operation-failed")(1,"p"),s(2),l()()),t&2&&(o(2),w(n))}function $e(t,n){if(t&1){let e=x();a(0,"app-quiz-finished-message",5),y("seeResults",function(){f(e);let r=u();return v(r.goToResults())}),l()}}function Ue(t,n){t&1&&(a(0,"app-operation-failed")(1,"p"),s(2),l()()),t&2&&(o(2),w(n))}function We(t,n){t&1&&(a(0,"app-operation-failed")(1,"p"),s(2),l()()),t&2&&(o(2),w(n))}function Je(t,n){t&1&&s(0),t&2&&B(" ",n," ")}function Ke(t,n){t&1&&s(0," Save Answer ")}function Xe(t,n){if(t&1){let e=x();g(0,"app-quiz-attempt-header"),h(1,Ue,3,1,"app-operation-failed"),h(2,We,3,1,"app-operation-failed"),a(3,"div",6)(4,"div",7),g(5,"app-question-attempt-header",8),M(6,9),a(7,"app-navigation-buttons",10),y("previousButtonClicked",function(){f(e);let r=u();return v(r.quizAttemptStore.GoToPreviousQuestion())})("nextButtonClicked",function(){f(e);let r=u();return v(r.quizAttemptStore.GoToNextQuestion())}),l(),a(8,"button",11),y("click",function(){f(e);let r=u();return v(r.quizAttemptStore.saveCurrentAnswer())}),h(9,Je,1,1)(10,Ke,1,0),l()(),a(11,"aside",12),g(12,"app-questions-navigator")(13,"app-questions-progress-bar"),a(14,"button",13),y("click",function(){f(e);let r=u();return v(r.onSubmitQuiz())}),s(15," Submit Quiz "),l()()()}if(t&2){let e,i,r,d=u();o(),z((e=d.quizAttemptStore.error()("submit"))?1:-1,e),o(),z((i=d.quizAttemptStore.error()("start"))?2:-1,i);let S=d.quizAttemptStore.quizQuestions()[d.quizAttemptStore.currentQuestionIndex()];o(3),p("questionType",S.type),o(),p("ngComponentOutlet",d.questionMapperService.getSuitableQuestionAttemptComponent(S.type))("ngComponentOutletInputs",F(11,je,S)),o(),p("canGoPrevious",d.quizAttemptStore.canGoPrevious())("canGoNext",d.quizAttemptStore.canGoNext()),o(),p("loading",d.quizAttemptStore.isPending()("submit-answer"))("disabled",!d.quizAttemptStore.currentAnswerDraft()||d.quizAttemptStore.quizTimeOut()),o(),z((r=d.savedLabel())?9:10,r),o(5),p("loading",d.quizAttemptStore.isPending()("submit"))}}function Ze(t,n){if(t&1){let e=x();a(0,"app-confirm-action-modal",14),y("confirmed",function(){f(e);let r=u();return v(r.onLeave(!0))})("cancelled",function(){f(e);let r=u();return v(r.onLeave(!1))}),l()}}function et(t,n){if(t&1){let e=x();a(0,"app-confirm-action-modal",15),y("confirmed",function(){f(e);let r=u();return v(r.onConfirmSubmit())})("cancelled",function(){f(e);let r=u();return v(r.showSubmitConfirmModal.set(!1))}),l()}}var Oe=class t{questionMapperService=m(H);quizId=V.required();quizAttemptStore=m(C);router=m(_e);route=m(be);showLeaveConfirmModal=W(!1);showSubmitConfirmModal=W(!1);resolveLeave=null;savedLabel=Q(()=>{let n=this.quizAttemptStore.lastSavedAt();return!n||Date.now()-n>3e3?null:"\u2713 Saved"});attemptId=xe(this.route.queryParamMap.pipe(ee(n=>n.get("attemptId"))));ngOnInit(){this.quizAttemptStore.load({quizId:this.quizId(),attemptId:this.attemptId()})}unloadNotification(n){this.isQuizInProgress()&&n.preventDefault()}canDeactivate(){return this.isQuizInProgress()?(this.showLeaveConfirmModal.set(!0),new Promise(n=>{this.resolveLeave=n})):!0}onLeave(n){this.showLeaveConfirmModal.set(!1),this.resolveLeave?.(n),this.resolveLeave=null}onSubmitQuiz(){this.showSubmitConfirmModal.set(!0)}onConfirmSubmit(){this.showSubmitConfirmModal.set(!1),this.quizAttemptStore.completeAttempt()}goToResults(){this.router.navigate(["/student/results"])}isQuizInProgress(){let n=this.quizAttemptStore,e=!n.isPending()("load")&&!n.error()("load"),i=n.isFulfilled()("submit");return e&&!i}static \u0275fac=function(e){return new(e||t)};static \u0275cmp=c({type:t,selectors:[["app-quiz-attempt"]],hostBindings:function(e,i){e&1&&y("beforeunload",function(d){return i.unloadNotification(d)},ie)},inputs:{quizId:[1,"quizId"]},features:[N([C])],decls:7,vars:3,consts:[["aria-label","Quiz attempt layout",1,"attempt-layout"],[1,"spinner-container"],["title","Leave Quiz","warningMessage","Are you sure you want to leave? Your progress will be saved.","confirmationPhrase","leave","confirmButtonText","I understand, leave","variant","danger"],["title","Submit Quiz","warningMessage","Are you sure you want to submit your quiz? You will not be able to edit your answers after this.","confirmationPhrase","submit","confirmButtonText","Yes, Submit Quiz","variant","success"],["ariaLabel","Loading quiz attempt"],[3,"seeResults"],[1,"attempt-main"],["aria-label","Question area",1,"question-column"],[3,"questionType"],[3,"ngComponentOutlet","ngComponentOutletInputs"],["ariaLabel","Question navigation",3,"previousButtonClicked","nextButtonClicked","canGoPrevious","canGoNext"],["appButton","","variant","green","type","button",3,"click","loading","disabled"],["aria-label","Quiz tools",1,"sidebar-column"],["appButton","","variant","red","type","button",2,"width","100%",3,"click","loading"],["title","Leave Quiz","warningMessage","Are you sure you want to leave? Your progress will be saved.","confirmationPhrase","leave","confirmButtonText","I understand, leave","variant","danger",3,"confirmed","cancelled"],["title","Submit Quiz","warningMessage","Are you sure you want to submit your quiz? You will not be able to edit your answers after this.","confirmationPhrase","submit","confirmButtonText","Yes, Submit Quiz","variant","success",3,"confirmed","cancelled"]],template:function(e,i){if(e&1&&(a(0,"section",0),h(1,Ge,2,0,"div",1)(2,Ye,3,1,"app-operation-failed")(3,$e,1,0,"app-quiz-finished-message")(4,Xe,16,13),h(5,Ze,1,0,"app-confirm-action-modal",2),h(6,et,1,0,"app-confirm-action-modal",3),l()),e&2){let r;o(),z(i.quizAttemptStore.isPending()("load")?1:(r=i.quizAttemptStore.error()("load"))?2:i.quizAttemptStore.isFulfilled()("submit")?3:4,r),o(4),z(i.showLeaveConfirmModal()?5:-1),o(),z(i.showSubmitConfirmModal()?6:-1)}},dependencies:[$,G,Qe,j,L,Y,we,qe,R,U,Me],styles:["[_nghost-%COMP%]{display:block;padding:1rem}.attempt-layout[_ngcontent-%COMP%]{display:grid;gap:1rem;width:min(100%,70rem);margin:0 auto}.attempt-main[_ngcontent-%COMP%]{display:grid;gap:1rem;grid-template-columns:2fr 1fr;align-items:start}.question-column[_ngcontent-%COMP%], .sidebar-column[_ngcontent-%COMP%]{display:grid;gap:1rem}.spinner-container[_ngcontent-%COMP%]{display:flex;align-items:center;justify-content:center;min-height:20rem}@media(width<=64rem){.attempt-main[_ngcontent-%COMP%]{grid-template-columns:1fr}}"],changeDetection:0})};export{Oe as QuizAttempt};
