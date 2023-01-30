import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
    selector: '[skill]',
})
export class DisallowSpecialDirective {
    constructor(private el: ElementRef) { }

    @HostListener('keyup', ['$event']) keyup(event) {
        event.target['value'] = this.el.nativeElement.value.replace(
            /[&\/\\^,+()_@$;!~%\[\]'":*?<>{}=]/g,
            ''
        );
    }
}
