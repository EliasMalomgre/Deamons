package be.kdg.stemtest.model.repositories

import android.util.Log
import androidx.lifecycle.LiveData
import androidx.lifecycle.LiveDataReactiveStreams
import be.kdg.stemtest.model.Interactors.StatementDatabaseInteractor
import be.kdg.stemtest.model.Interactors.StatementRemoteInteractor
import be.kdg.stemtest.model.entity.*
import io.reactivex.*
import io.reactivex.Observable
import io.reactivex.android.schedulers.AndroidSchedulers
import io.reactivex.schedulers.Schedulers
import java.util.concurrent.TimeUnit
import javax.inject.Inject

class StatementRepository @Inject constructor(
    private val databaseInteractor: StatementDatabaseInteractor,
    private val remoteInteractor: StatementRemoteInteractor
) {

    fun getStatement(index: Int): LiveData<Statement> {
        val database = databaseInteractor.getData(index).toObservable()
        val remote = remoteInteractor.getStatement(index).toObservable()

        val observable = Observable.concat(database,remote)
            .onErrorReturn { Statement(-1,"error",null) }
            .retryWhen { i -> i.delay(4,TimeUnit.SECONDS)  }
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
        return LiveDataReactiveStreams.fromPublisher(observable.toFlowable(BackpressureStrategy.ERROR))
    }

    fun getAnswerOptions(index: Int): LiveData<List<AnswerOption>>{
        val database = databaseInteractor.getAnswerOptions(index)
        return LiveDataReactiveStreams.fromPublisher(database)
    }


    fun pushAnswer(argument: String?, answerId:Int, index: Int) {
       val database= databaseInteractor.sendAnswer(StudentAnswer(index+1,argument,answerId,null))
        val remote = remoteInteractor.pushAnswer(argument,answerId)
            .doOnComplete { Log.i("Daemonic messenger", "Answer send") }
            .doOnError{ Log.i("Daemonic messenger","Answer error")}
            .retryWhen { i -> i.delay(4,TimeUnit.SECONDS)  }


        database.mergeWith(remote)
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
            .subscribe()

    }



    fun getIndex(): Int? {
       return databaseInteractor
           .getMax()
           .subscribeOn(Schedulers.io())
           .observeOn(Schedulers.io())
           .blockingGet()
    }

    fun getAllAnswerOptions(): LiveData<List<AnswerOption>> {
        val database= databaseInteractor.getAllAnswerOptions()
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
        return LiveDataReactiveStreams.fromPublisher(database.toFlowable())

    }

    fun getAllStatements(): LiveData<List<Statement>> {
        val database = databaseInteractor.getAllStatements()
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
        return LiveDataReactiveStreams.fromPublisher(database.toFlowable())

    }

    fun getAllDefinitions(): LiveData<List<Definition>> {
        val database = databaseInteractor.getAllDefinitions()
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())

        return LiveDataReactiveStreams.fromPublisher(database.toFlowable())
    }


}

