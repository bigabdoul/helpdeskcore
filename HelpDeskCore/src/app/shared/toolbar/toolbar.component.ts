import { Component, OnInit } from '@angular/core';
import { SimpleDictionary } from "../utils/dictionary";

@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss']
})
export class ToolbarComponent implements OnInit {

  popups: SimpleDictionary<string, boolean> = new SimpleDictionary<string, boolean>(
    { key: 'replyallmenu', value: false },
    { key: 'forwardmenu', value: false },
    { key: 'markmessagemenu', value: false },
    { key: 'messagemenu', value: false },
  );
  
  constructor() {
  }

  ngOnInit() {
  }
  
  showPopup(name: string) {
    this.popups.all(item => item.value = false);
    this.popups.get(name, item => {
      item.value = true;
      console.log('popup found: ', name);
    });
    return false;
  }

  shown(name: string) {
    const kvp = this.popups.get(name);
    return kvp && kvp.value;
  }

  command(name: string, arg?: string) {
    name = name || '';
    console.log('command: ', name, arg);
    switch (name) {
      case 'checkmail':
        break;
      case 'reply':
        break;
      case 'reply-all':
        break;
      case 'reply-list':
        break;
      case 'forward':
        break;
      case 'forward-inline':
        break;
      case 'forward-attachment':
        break;
      case 'open':
        break;
      case 'edit':
        break;
      case 'delete':
        break;
      case 'mark':
        break;
      case 'print':
        break;
      case 'download':
        break;
      case 'viewsource':
        break;
      default:
    }
    return false;
  }
}
