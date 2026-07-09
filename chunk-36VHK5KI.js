import{ea as s,ha as C,ja as I,ka as M,la as r}from"./chunk-67O4C7MM.js";import{o as S}from"./chunk-IXU7L534.js";import{M as c,Ma as u,Mb as k,N as p,Na as f,Nb as b,P as m,Qa as h,R as o,Ra as g,Yb as v,bb as y,la as d}from"./chunk-N3IKTFHA.js";import{a,b as l}from"./chunk-HIUAR4UN.js";var w=`
    .p-skeleton {
        display: block;
        overflow: hidden;
        background: dt('skeleton.background');
        border-radius: dt('skeleton.border.radius');
    }

    .p-skeleton::after {
        content: '';
        animation: p-skeleton-animation 1.2s infinite;
        height: 100%;
        left: 0;
        position: absolute;
        right: 0;
        top: 0;
        transform: translateX(-100%);
        z-index: 1;
        background: linear-gradient(90deg, rgba(255, 255, 255, 0), dt('skeleton.animation.background'), rgba(255, 255, 255, 0));
    }

    [dir='rtl'] .p-skeleton::after {
        animation-name: p-skeleton-animation-rtl;
    }

    .p-skeleton-circle {
        border-radius: 50%;
    }

    .p-skeleton-animation-none::after {
        animation: none;
    }

    @keyframes p-skeleton-animation {
        from {
            transform: translateX(-100%);
        }
        to {
            transform: translateX(100%);
        }
    }

    @keyframes p-skeleton-animation-rtl {
        from {
            transform: translateX(100%);
        }
        to {
            transform: translateX(-100%);
        }
    }
`;var E={root:{position:"relative"}},F={root:({instance:e})=>["p-skeleton p-component",{"p-skeleton-circle":e.shape==="circle","p-skeleton-animation-none":e.animation==="none"}]},D=(()=>{class e extends C{name="skeleton";style=w;classes=F;inlineStyles=E;static \u0275fac=(()=>{let t;return function(i){return(t||(t=d(e)))(i||e)}})();static \u0275prov=c({token:e,factory:e.\u0275fac})}return e})();var N=new m("SKELETON_INSTANCE"),B=(()=>{class e extends M{componentName="Skeleton";$pcSkeleton=o(N,{optional:!0,skipSelf:!0})??void 0;bindDirectiveInstance=o(r,{self:!0});onAfterViewChecked(){this.bindDirectiveInstance.setAttrs(this.ptms(["host","root"]))}styleClass;shape="rectangle";animation="wave";borderRadius;size;width="100%";height="1rem";_componentStyle=o(D);get containerStyle(){let t=this._componentStyle?.inlineStyles.root,n;return this.$unstyled()||(this.size?n=l(a({},t),{width:this.size,height:this.size,borderRadius:this.borderRadius}):n=l(a({},t),{width:this.width,height:this.height,borderRadius:this.borderRadius})),n}get dataP(){return this.cn({[this.shape]:this.shape})}static \u0275fac=(()=>{let t;return function(i){return(t||(t=d(e)))(i||e)}})();static \u0275cmp=u({type:e,selectors:[["p-skeleton"]],hostVars:6,hostBindings:function(n,i){n&2&&(y("aria-hidden",!0)("data-p",i.dataP),k(i.containerStyle),b(i.cn(i.cx("root"),i.styleClass)))},inputs:{styleClass:"styleClass",shape:"shape",animation:"animation",borderRadius:"borderRadius",size:"size",width:"width",height:"height"},features:[v([D,{provide:N,useExisting:e},{provide:I,useExisting:e}]),h([r]),g],decls:0,vars:0,template:function(n,i){},dependencies:[S,s],encapsulation:2,changeDetection:0})}return e})(),G=(()=>{class e{static \u0275fac=function(n){return new(n||e)};static \u0275mod=f({type:e});static \u0275inj=p({imports:[B,s,s]})}return e})();export{B as a,G as b};
