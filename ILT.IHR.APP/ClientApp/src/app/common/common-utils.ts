import { FormControl, FormGroup } from "@angular/forms";

export class CommonUtils {

  dateInterval: any = [{ id: 0, name: 'Today' },
  { id: 1, name: 'Yesterday' },
  { id: 2, name: 'Last Week' },
  { id: 3, name: 'Last 2 Weeks' },
  { id: 4, name: 'Last 30 Days' },
  { id: 5, name: 'Last 6 Months' },
  { id: 6, name: 'Last 12 Months' },
  { id: 7, name: 'Current Month' },
  { id: 8, name: 'Current Year' },
  { id: 9, name: 'Previous Month' },
  { id: 10, name: 'Previous Year' },
  { id: 11, name: 'Date Range' },
  { id: 12, name: 'Month And Year' }];

  monthList: any = [{ id: 1, month: 'January' },
  { id: 2, month: 'February' },
  { id: 3, month: 'March' },
  { id: 4, month: 'April' },
  { id: 5, month: 'May' },
  { id: 6, month: 'June' },
  { id: 7, month: 'July' },
  { id: 8, month: 'August' },
  { id: 9, month: 'September' },
  { id: 10, month: 'October' },
  { id: 11, month: 'November' },
  { id: 12, month: 'December' }];

  formatDateDefault(date) {
    const d = new Date(date);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    const year = d.getFullYear();
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    var getDate = [month, day, year].join('/');
    if (getDate === '01/01/1900') {
      return ''
    } else if (getDate === '01/01/1') {
      return ''
    } else {
      return getDate;
    }
  }

  defaultDateFormat(date) {
    const d = new Date(date);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    const year = d.getFullYear();
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    return [year, month, day].join('-');
  }

  defaultDateTimeLocalSet(date) {
    if(date === ''){
      return '';
    } else {
      var now = new Date(date);
      var year = now.getFullYear();
      var month = now.getMonth() + 1;
      var day = now.getDate();
      var hour = now.getHours();
      var minute = now.getMinutes();
      var localDatetime = year + "-" +
        (month < 10 ? "0" + month.toString() : month) + "-" +
        (day < 10 ? "0" + day.toString() : day) + "T" +
        (hour < 10 ? "0" + hour.toString() : hour) + ":" +
        (minute < 10 ? "0" + minute.toString() : minute) + ":00.106Z";
      return localDatetime;
    }
  }
  defaultDateAndTime() {
    var now = new Date();
    var utcString = now.toISOString().substring(0, 19);
    var year = now.getFullYear();
    var month = now.getMonth() + 1;
    var day = now.getDate();
    var hour = now.getHours();
    var minute = now.getMinutes();
    var seconds = now.getSeconds();
    var localDatetime = (month < 10 ? "0" + month.toString() : month) + "/" +
      (day < 10 ? "0" + day.toString() : day) + "/" +
      year + " " +
      (hour < 10 ? "0" + hour.toString() : hour) + ":" +
      (minute < 10 ? "0" + minute.toString() : minute) + ":" +
      (seconds < 10 ? "0" + minute.toString() : seconds)
    return localDatetime;
  }

  sevenDaysBeforeDate(today) {
    var date = new Date(today);
    date.setDate(date.getDate() - 7);
    let month = '' + (date.getMonth() + 1);
    let day = '' + date.getDate();
    const year = date.getFullYear();
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    return [year, month, day].join('-');
  }

  fiftyYearsBeforeDate(today) {
    var date = new Date(today);
    date.setDate(date.getDate());
    let month = '' + (date.getMonth() + 1);
    let day = '' + date.getDate();
    const year = (date.getFullYear() - 50);
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    return [year, month, day].join('-');
  }

  formatDateWithAmPM(date) {
    const d = new Date(date);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    const year = d.getFullYear();
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    var getDate = [month, day, year].join('/');
    var hours = d.getHours();
    let minutes = d.getMinutes().toString();
    if(minutes.length === 1){
      minutes = '0' + minutes;
    }
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    let hour = hours.toString();
    if(hour.length === 1) {
      hour = '0' + hour;
    }
    var strTime = hour + ':' + minutes + ' ' + ampm;
    const dateTime = getDate + ' ' +strTime;
    if(dateTime != '01/01/1 12:0 am'){
      return dateTime
    } else {
      return '';
    }
    
  }

  formatDateTimeWithAmPM(date) {
    const d = new Date(date);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    const year = d.getFullYear();
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    var getDate = [month, day, year].join('/');
    var hours = d.getHours();
    let minutes = d.getMinutes().toString();
    let seconds = d.getSeconds().toString();
    if(minutes.length === 1){
      minutes = '0' + minutes;
    }
    if(seconds.length === 1){
      seconds = '0' + seconds;
    }
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    let hour = hours.toString();
    if(hour.length === 1) {
      hour = '0' + hour;
    }
    var strTime = hour + ':' + minutes + ':' + seconds + ' ' + ampm;
    const dateTime = getDate + ' ' + strTime;
    if(dateTime === '01/01/1900 12:00:00 AM'){
        return '';
    } else if (dateTime === '01/01/1 12:00:00 AM') {
        return '';
    }
    else {
      return dateTime
    }
    
    }
    SearchFunction() {
        var input, filter, cards, cardContainer, title, i;
        input = document.getElementById("myFilter");
        filter = input.value.toUpperCase();
        cardContainer = document.getElementById("myItems");
        cards = cardContainer.getElementsByClassName("card");
        for (i = 0; i < cards.length; i++) {
            title = cards[i].querySelector(".card-body");
            if (title.innerText.toUpperCase().indexOf(filter) > -1) {
                cards[i].style.display = "";
            } else {
                cards[i].style.display = "none";
            }
        }
    }

  // 2020-05-22
  formatDatePickerDate(dateObject) {
    if (dateObject != undefined) {
      var date = [dateObject.year, dateObject.month, dateObject.day].join('-');
      return date;
    } else {
      return '';
    }
  }

  getYearList() {
    var date = new Date,
      years = [],
      year = date.getFullYear();
    for (var i = year - 14; i <= year; i++) {
      years.push({ id: i, year: i });
    }
    return years;
  }

  getTodaysMonth() {
    const month = new Date().getMonth() + 1;
    return month;
  }

  getTodaysDay() {
    const day = new Date().getDate();
    return day;
  }
  
  getTodaysYear() {
    const year = new Date().getFullYear();
    return year;
  }
  getBeforeSevenDaysMonth() {
    let date = new Date();
    date.setDate(date.getDate() - 7);
    return date.getMonth() + 1;
  }

  getBeforeSevenDaysDay() {
    let date = new Date();
    date.setDate(date.getDate() - 7);
    return date.getDate();
  }
  
  getBeforeSevenDaysYear() {
    let date = new Date();
    date.setDate(date.getDate() - 7);
    return date.getFullYear();
  }
    getNoFeaturesDates() {
        let date = new Date();
        return date
    }
  getMaxDate() {
    let date = new Date();
    return {year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDate()}
  }

  formatToDatePicker(date) {
    let date1 = new Date(date);
    return { year: date1.getFullYear(), month: date1.getMonth() + 1, day: date1.getDate() }
  }

  setSevenDayBeforeDate() {
    var date = new Date();
    date.setDate(date.getDate() - 7);
    let month = '' + (date.getMonth() + 1);
    let day = '' + date.getDate();
    const year = date.getFullYear();
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    return [month, day, year].join('/');
  }

  getDateRange() {
    let date = new Date();
    const year = (date.getFullYear() - 130);
    const currentYear = (date.getFullYear());
    return `${year}:${currentYear}`;
  }

  public noWhitespaceValidator = (control: FormControl) => {
    const isWhitespace = (control.value || '').trim().length === 0;
    const isValid = !isWhitespace;
     return isValid ? null : { 'whitespace': true };
}

public mask = {
  guide: false,
  showMask: false,
  mask: ['(',/\d/,/\d/,/\d/,')',/\d/,/\d/,/\d/, '-', /\d/, /\d/, /\d/, /\d/]
};
public faxMask = {
  guide: false,
  showMask: false,
  mask: ['(',/\d/,/\d/,/\d/,')',' ',/\d/,/\d/,/\d/, '-', /\d/, /\d/, /\d/, /\d/]
};
public ssnMask = {
  guide: false,
  showMask: false,
  mask: [/\d/,/\d/,/\d/,'-',/\d/,/\d/, '-', /\d/, /\d/, /\d/, /\d/]
    };

  newGuid() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0,
                v = c == 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }

    setDropdownvalue(list: any[], findValue: any, selectedValue: any, Form: FormGroup, fieldName: any, fieldValue: any,isString: boolean = false) {
        if (list !== undefined && list !== null && list.length > 0 && selectedValue != undefined && selectedValue != null && selectedValue != "" && selectedValue.value !== undefined && selectedValue.value !== null && selectedValue.value !== "" && findValue !== null && findValue !== 0 && findValue !== "" && findValue !== undefined) {
            if (isString === true) {
                let selectedItem = list.find((item) => item[findValue].toLowerCase() === selectedValue.value.toLowerCase());
                if (selectedItem !== undefined && selectedItem !== null) {
                    var isKeyExist: boolean = Object.keys(selectedItem).some(v => v === fieldValue);
                    if (isKeyExist === true) {
                        Form.controls.fieldName.patchValue(selectedItem[fieldValue])
                    }
                }
            } else {
                let selectedItem = list.find((item) => item[findValue] === selectedValue.value);
                if (selectedItem !== undefined && selectedItem !== null) {
                    var isKeyExist: boolean = Object.keys(selectedItem).some(v => v === fieldValue);
                    if (isKeyExist === true) {
                        Form.controls[fieldName].patchValue(selectedItem[fieldValue])
                    }
                }
            }
         }
    }

}
