import { Component, OnInit, Input, SimpleChanges, OnChanges, Renderer2, ElementRef,
  Output, 
  EventEmitter,
  ViewChild} from '@angular/core';
import { ITableRowAction, ITableHeaderAction } from './table-options';
import { SortEvent } from 'primeng/api/sortevent';
import * as moment from 'moment';
import { NgxSpinnerService } from 'ngx-spinner';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { EmployeeAssignmentService } from '../../pages/employee/employee-assignment/assignment.service';
import { IAssignmentRateDisplay } from '../../core/interfaces/AssignmentRate';

@Component({
  selector: 'app-ihr-table',
  templateUrl: './ihr-table.component.html',
    styleUrls: ['./ihr-table.component.scss'],
    animations: [
        trigger('rowExpansionTrigger', [
            state('void', style({
                transform: 'translateX(-10%)',
                opacity: 0
            })),
            state('active', style({
                transform: 'translateX(0)',
                opacity: 1
            })),
            transition('* <=> *', animate('400ms cubic-bezier(0.86, 0, 0.07, 1)'))
        ])
    ]
})

export class IHRTableComponent implements OnInit {
  @ViewChild('dt', { static: false }) dt: any;
  @Input() data: any[] = [];
  @Input() isPagination: boolean = true;
  @Input() loading: boolean = true;
  @Input() showCurrentPageReport: boolean = true;
  @Input() cols: any[] = [];
  @Input() tableState: string = '';
  @Input() selectedColumns: any[];
  @Input() isRowAction: boolean = false;
  @Input() rowActions: ITableRowAction[] = [];
  @Input() headerActions: ITableHeaderAction[] = [];
  @Input() selectedRows: any[] = []; 
  @Input() isRowSelection: boolean = false;
  @Input() selectionMode: string = 'single';
  @Output() onRowSelected: EventEmitter<any> =  new EventEmitter<any>();
  @Output() onNumberChange: EventEmitter<any> =  new EventEmitter<any>();
  @Output() onCheckBoxChange: EventEmitter<any> =  new EventEmitter<any>();
  @Output() onFilter: EventEmitter<any> =  new EventEmitter<any>();
  @Output() lstSelectedColumns: EventEmitter<any[]> = new EventEmitter<any[]>(); 
  // 
  @Input() isTableCheckbox: boolean = true;
  @Input() sortingField: any;
  @Input() sortingFieldOrder: number = 1;
  @Input() disabledRow: boolean = false;
  @Input() isSearchRequired: boolean = false;
  @Input() hasChanged: boolean = false;
  @Input() globalFilterFields: any[] = [];
  @Input() hasChange: boolean = false;
  @Input() defaultPageSize: number = 15;


 searchText: string = '';


  public dateFieldFormat:string = 'MM/DD/YYYY';

  // after search 1 dropdown or multiselect in Header
  @Input() DropDown: any[] = [];
  @Input() DropDownLabel: string = '';
  @Input() DefaultID: number = null;
  @Output() onDropDownChange: EventEmitter<any> =  new EventEmitter<any>();
  public OnDrpDwnChange(event)
  {
     // this.reset()
      //this.refresh();
      this.DefaultID = Number(event.value);
      this.onDropDownChange.emit(this.DefaultID);
  }
  @Input() onMultiSelectDropDown1: any[] = [];
  @Input() onMultiSelectedDropDown1: number[] = [];
  @Output() onMultiSelectDropDown1Change: EventEmitter<any> =  new EventEmitter<any>();
  OnMultiDropDown1Change(event: any) {

    this.onMultiSelectedDropDown1 = [];
    this.onMultiSelectedDropDown1 = event.value;
    this.onMultiSelectDropDown1Change.emit(this.onMultiSelectedDropDown1);
   // this.loadMobilesItems();
}

// 2nd dropdown part 
@Input() DropDown2: any[] = [];
@Input() DropDown2Label: string = '';
@Input() DropDown2DefaultID: number = null;
@Output() onDropDown2Change: EventEmitter<any> =  new EventEmitter<any>();
public OnDrpDwn2Change(event)
{
    this.reset()
    this.DropDown2DefaultID = Number(event.value);
    this.onDropDown2Change.emit(this.DropDown2DefaultID);
}
 @Input() onMultiSelectDropDown: any[] = [];
 @Input() onMultiSelectedDropDown: number[] = [];
 @Output() onMultiSelectDropDownChange: EventEmitter<any> =  new EventEmitter<any>();
 OnMultiDropDownChange(event: any) {
   this.onMultiSelectedDropDown = [];
   this.onMultiSelectedDropDown = event.value;
   this.onMultiSelectDropDownChange.emit(this.onMultiSelectedDropDown);
}

 @Input() DropDown3: any[] = [];
@Input() DropDown3Label: string = '';
@Input() DropDown3DefaultID: number = null;
@Output() onDropDown3Change: EventEmitter<any> =  new EventEmitter<any>();
public OnDrpDwn3Change(event)
{
    this.DropDown3DefaultID = Number(event.value);
    this.onDropDown3Change.emit(this.DropDown3DefaultID);
}




 @Input() get _selectedColumns(): any[] {
    return this.selectedColumns;
  }

  

set _selectedColumns(val: any[]) {
    this.selectedColumns = this.cols.filter(col => val.includes(col));
    // let currentState = localStorage.getItem(this.tableState);
    // console.log(currentState);
    // currentState['selection'] = this.selectedColumns.toString();
    // localStorage.removeItem(this.tableState);
    // localStorage.setItem(this.tableState, currentState);
    // console.log(localStorage.getItem(this.tableState));
  }

  first = 0;
  rows = 15;


    
    @Output() UpdatePageSize: EventEmitter<any> = new EventEmitter<any>();
    public tableUpdatePageSize(event: any) {
        this.defaultPageSize = 0;
        setTimeout(() => {
            this.defaultPageSize = event
        })
        this.UpdatePageSize.emit(this.defaultPageSize)
    }
    pagesSizes = [10, 15, 25, 50];
    temDataLength: number = 0;
  constructor(
    private renderer: Renderer2,
    private el: ElementRef,
      private ngxSpinner: NgxSpinnerService,
    private assignmentService: EmployeeAssignmentService
  ) { }

    ngOnInit(): void {
        this.defaultPageSize = 15;
        this.reset()
       // this.first = 0;
        //this.refresh();
        if (this.data !== null) {
            this.data.length < this.rows ? this.temDataLength = this.data.length : this.temDataLength = this.rows;
        }
    }


    ngAfterViewInit(): void {
        //this.reset();
       // this.first = 0;
      //  this.refresh();
        this.dt.filters = {};
       //  this.searchText = this.dt.filters != null && this.dt.filters.global != undefined ? this.dt.filters['global']['value'] : '';
        if (this.data !== null) {
            this.data.length < this.rows ? this.temDataLength = this.data.length : this.temDataLength = this.rows;
        }
    }

    ngAfterViewChecked(): void {

    }

  ngOnChanges(changes: SimpleChanges): void {
    // if(this.loading === true) {
    //   this.ngxSpinner.show();
    // }else if(this.loading === false) {
    //   this.ngxSpinner.hide()
    // }
  }

    next() {
    this.first = this.first + this.rows;
  }

    prev() {
    this.first = this.first - this.rows;
  }

    reset() {
        setTimeout(() => { this.first = 0;})
  }

  onSearch() {
   // this.onFilter.emit(this.searchText);
  }

  filterData(searchText: string){
    this.searchText = searchText;
    this.dt.filterGlobal(searchText, 'contains');
  }

  isLastPage(): boolean {
    return this.first === (this.data.length - this.rows);
  }

  isFirstPage(): boolean {
    return this.first === 0;
  }

  getRowActionToolTip(toolTip) {
    if (toolTip !== undefined && toolTip !== null) {
      return toolTip;
    }
    return '';
  }

  onRowSelect(event) {
    this.onRowSelected.emit(event.data);
  }

  numberChange(rowData) {
    this.onNumberChange.emit(rowData);
  }

  checkboxChange(rowData) {
    rowData.Active = !rowData.Active
    this.onCheckBoxChange.emit(rowData);
  }

    paginate(event) {
       console.log(event)
      //this.tableUpdatePageSize(event.rows)
      this.first = event.first;

    }

    onPageChange(event) {
      //  console.log(event)
    }
  refresh(){    
    this.dt._first = 0;
   //call your get service without filter
  }
  isDateColumn(event, columnTitle: string) {
    for (const row of event.data) {
      const value = row[columnTitle];
      if (!moment(value, this.dateFieldFormat).isValid() && value !== null) {
        return false;
      }
    }
    return true;
}

sortColumn(event: SortEvent) {
   event.data.sort((item1, item2) => {
    const value1: string = item1[event.field];
    const value2: string = item2[event.field];

    if (value1 === null) {
      return 1;
    }
    
    if (this.isDateColumn(event, event.field)) {
      const date1 = moment(value1, this.dateFieldFormat);
      const date2 = moment(value2, this.dateFieldFormat);
      let result: number = -1;
      if (moment(date2).isBefore(date1, 'day')) { result = 1; }
      return result * event.order;
    }

    let result = null;


    if (value1 == null && value2 != null) {
      result = -1;
    } else if (value1 != null && value2 == null) {
      result = 1;
    } else if (value1 == null && value2 == null) {
      result = 0;
    } else if (typeof value1 === 'string' && typeof value2 === 'string') {
      result = value1.localeCompare(value2);
    } else {
      result = (value1 < value2) ? -1 : (value1 > value2) ? 1 : 0;
    }

    return (event.order * result);
  });
}
getpageReport() {
  if (this.data.length > 0) {
    return 'Showing {first} to {last} of {totalRecords} entries'
  } else {
    return 'Showing 0 to {last} of {totalRecords} entries'
  }
    }
    isExpanded: boolean = false;
    @Input() dataKey: number;
    expandedRows: any = {};
    @Input() isRowExpand: boolean = false;
    @Input() isRowExpandContact: boolean = false;
    @Input() isExpandRowRate: boolean = false;


    assignmenRates: IAssignmentRateDisplay [] = [];
    isOpen(rowData: any) {
        // console.log(this.isExpandRowRate);
        if (this.isExpandRowRate === true) {
            this.assignmentService.getEmployeeAssignmentById(rowData.assignmentID).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                    var assignment = result['data'];
                    if (assignment.assignmentRates.length > 0) {
                        assignment.assignmentRates.forEach((d) => {
                            d.startDate = moment(d.startDate).format("MM/DD/YYYY")
                            if (d.endDate !== null) {
                                d.endDate = moment(d.endDate).format("MM/DD/YYYY")
                            }
                        })
                    }
                    this.data2 = assignment.assignmentRates !== null || assignment.assignmentRates.length > 0 ? assignment.assignmentRates : [];
                    this.assignmenRates = assignment.assignmentRates;
                }
            })
        }
    }

    @ViewChild('sdt', { static: false }) sdt: any;
    @Input() data2: any[] = [];
    @Input() isChildTablePagination: boolean = true;
    @Input() IsloadingChild: boolean = true;
    @Input() showCurrentChildPageReport: boolean = true;
    @Input() childCols: any[] = [];
    @Input() tableChildState: string = '';
    @Input() selectedChildColumns: any[];
    @Input() isChildRowAction: boolean = false;
    @Input() rowChildActions: ITableRowAction[] = [];
    @Input() headerChildActions: ITableHeaderAction[] = [];

    @Input() get _selectedChildColumns(): any[] {
        return this.selectedChildColumns;
    }
    set _selectedChildColumns(val: any[]) {
        this.selectedChildColumns = this.childCols.filter(col => val.includes(col));
    }


    @Output() isOpenTicketForm: EventEmitter<any> = new EventEmitter<any>();
    ticketList(data: any) {
        console.log(data)
        this.isOpenTicketForm.emit(data)
    }

// header dropdown and multiselect dropdown onchange methods
}

