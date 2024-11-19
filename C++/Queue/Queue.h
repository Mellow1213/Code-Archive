#ifndef QUEUE_H
#define QUEUE_H

class Queue
{
    int *arr;
    inline void moveNext(int &ptr) { ptr = (ptr + 1) % capacity_; }
    int size_;
    int capacity_;
    int front_;
    int rear_;

public:
    Queue(int capacity);
    ~Queue();
    void push(int element);
    int pop();
    int size();
    int front();
    int back();
    bool empty();
    void reserve(int capacity);
};

#endif