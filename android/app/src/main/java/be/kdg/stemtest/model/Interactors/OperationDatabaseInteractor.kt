package be.kdg.stemtest.model.Interactors

import be.kdg.stemtest.model.db.StemTestDb
import io.reactivex.Completable
import io.reactivex.Scheduler
import io.reactivex.android.schedulers.AndroidSchedulers
import io.reactivex.schedulers.Schedulers
import javax.inject.Inject

class OperationDatabaseInteractor @Inject constructor(private val stemTestDb: StemTestDb) {


    fun deleteALL(){
        Completable.create{ stemTestDb.clearAllTables()}
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
            .subscribe()
    }
}